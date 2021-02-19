﻿using UnityEngine;
using System.Collections;
using System.Net;
using ubv.udp;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ubv.common.data;

namespace ubv
{
    namespace client
    {
        namespace logic
        {
            abstract public class ClientSyncState
            {
                protected readonly tcp.client.TCPClient m_TCPClient;
                protected readonly udp.client.UDPClient m_UDPClient;

                public ClientSyncState(tcp.client.TCPClient tcpClient, udp.client.UDPClient udpClient)
                {
                    m_TCPClient = tcpClient;
                    m_UDPClient = udpClient;
                }

                protected readonly object m_lock = new object();
                
                public abstract ClientSyncState Update();
                public abstract ClientSyncState FixedUpdate();
            }

            public class ClientSyncInit : ClientSyncState, tcp.client.ITCPClientReceiver
            {
                private readonly world.WorldRebuilder m_worldRebuilder;
                private bool m_readyToPlay;
                private common.world.cellType.CellInfo[,] m_cellInfos;

                //tranfer to play
                private readonly string m_physicsScene;
                private readonly PlayerSettings m_playerSettings;
                private List<PlayerState> m_playerStates;
                private int? m_playerID;
                private int m_simulationBuffer;

#if NETWORK_SIMULATE
                private bool m_playWithoutServer;
#endif // NETWORK_SIMULATE

                public ClientSyncInit( 
                    tcp.client.TCPClient TCPClient, 
                    udp.client.UDPClient UDPClient,
                    string physicsScene, 
                    PlayerSettings playerSettings,
                    world.WorldRebuilder worldGrid
#if NETWORK_SIMULATE
                    , ClientSync parent) 
#endif // NETWORK_SIMULATE
                    : base(TCPClient, UDPClient)
                {
                    m_physicsScene = physicsScene;
                    m_playerID = null;
                    m_playerStates = null;
                    m_readyToPlay = false;

                    m_playerSettings = playerSettings;

                    m_playWithoutServer = false;
                    
                    m_TCPClient.Subscribe(this);

                    m_worldRebuilder = worldGrid;

                    m_cellInfos = null;

                    m_worldRebuilder.OnWorldBuilt(OnWorldBuilt);

#if NETWORK_SIMULATE
                    parent.ConnectButtonEvent.AddListener(SendConnectionRequestToServer);
                    parent.PlayWithoutServerButtonEvent.AddListener(() => { m_playWithoutServer = true; });
#endif // NETWORK_SIMULATE
                }

                public override ClientSyncState FixedUpdate()
                {
                    return this;
                }

                public void SendConnectionRequestToServer()
                {
                    m_TCPClient.Connect();

                    m_TCPClient.Send(new IdentificationMessage(0).GetBytes()); // sends a ping to the server
                }

                public void ReceivePacket(tcp.TCPToolkit.Packet packet)
                {
                    // receive auth message and set player id
                    
                    IdentificationMessage auth = common.serialization.IConvertible.CreateFromBytes<IdentificationMessage>(packet.Data);
                    if (auth != null)
                    {
                        m_playerID = auth.PlayerID.Value;
#if DEBUG_LOG
                        Debug.Log("Received connection confirmation, player ID is " + m_playerID);
#endif // DEBUG_LOG

                        // send a ping to the server to make it known
                        m_UDPClient.Send(UDPToolkit.Packet.PacketFromBytes(auth.GetBytes()).RawBytes);
                    }
                    else
                    {
                        GameStartMessage start = common.serialization.IConvertible.CreateFromBytes<GameStartMessage>(packet.Data);
                        if (start != null)
                        {
                            m_playerStates = start.Players.Value;
                            m_simulationBuffer = start.SimulationBuffer.Value;
#if DEBUG_LOG
                            Debug.Log("Client received confirmation that server is about to start game with " + m_playerStates.Count + " players and " + m_simulationBuffer + " simulation buffer ticks");
#endif // DEBUG_LOG

                            m_worldRebuilder.BuildWorldFromCellInfo(start.CellInfo2DArray.Value);
                        }
                    }
                }

                private void OnWorldBuilt()
                {
                    m_TCPClient.Send(new ClientReadyMessage(m_playerID.Value).GetBytes());
                    m_readyToPlay = true;
                }

                public override ClientSyncState Update()
                {
                    if(m_readyToPlay)
                    {
                        return new ClientSyncPlay(m_UDPClient, m_playerID.Value, m_physicsScene, m_playerSettings, m_playerStates, m_simulationBuffer);
                    }
#if NETWORK_SIMULATE
                    if(m_playWithoutServer)
                    {
                        m_TCPClient.Unsubscribe(this);
                        PlayerState soloPlayer = new PlayerState();
                        soloPlayer.GUID.Value = 0;
                        List<PlayerState> players = new List<PlayerState>();
                        players.Add(soloPlayer);
                        return new ClientSyncPlay(m_UDPClient, 0, m_physicsScene, m_playerSettings, players, 0, m_playWithoutServer);
                    }
#endif // NETWORK_SIMULATE

                    return this;
                }
            }

            /// <summary>
            /// Represents the state of the server during the game
            /// </summary>
            public class ClientSyncPlay : ClientSyncState, udp.client.IUDPClientReceiver
            {
                private readonly int m_playerID;

                private uint m_remoteTick;
                private uint m_localTick;
                private readonly int m_simulationBuffer;

                private const ushort CLIENT_STATE_BUFFER_SIZE = 64;

                private ClientState[] m_clientStateBuffer;
                private InputFrame[] m_inputBuffer;
                private InputFrame m_lastInput;
                private ClientState m_lastServerState;

                private PhysicsScene2D m_clientPhysics;

                private List<IClientStateUpdater> m_updaters;
                
#if NETWORK_SIMULATE
                private readonly float m_packetLossChance = 0.15f;
                private readonly bool m_noServer;
#endif // NETWORK_SIMULATE

                public ClientSyncPlay(udp.client.UDPClient UDPClient, 
                    int playerID, 
                    string physicsScene, 
                    PlayerSettings playerSettings,
                    List<PlayerState> playerStates,
                    int simulationBuffer
#if NETWORK_SIMULATE
                    , bool startWithoutServer = false
#endif // NETWORK_SIMULATE
                    ) : base(null, UDPClient)
                {
#if NETWORK_SIMULATE
                    m_noServer = startWithoutServer;
#endif // NETWORK_SIMULATE
                    m_playerID = playerID;
                    m_localTick = 0;
                    m_clientStateBuffer = new ClientState[CLIENT_STATE_BUFFER_SIZE];
                    m_inputBuffer = new InputFrame[CLIENT_STATE_BUFFER_SIZE];

                    m_clientPhysics = SceneManager.GetSceneByName(physicsScene).GetPhysicsScene2D();
                    m_lastServerState = null;

                    m_updaters = new List<IClientStateUpdater>();
                    m_simulationBuffer = simulationBuffer;

                    Dictionary<int, PlayerState> playerStateDict = new Dictionary<int, PlayerState>();
                    foreach(PlayerState state in playerStates)
                    {
                        playerStateDict[state.GUID.Value] = state;
                    }
                    
                    m_updaters.Add(new PlayerGameObjectUpdater(playerSettings, playerStateDict, m_playerID));
                    
                    m_UDPClient.Subscribe(this);
                    
                    for (ushort i = 0; i < CLIENT_STATE_BUFFER_SIZE; i++)
                    {
                        m_clientStateBuffer[i] = new ClientState();
                        m_clientStateBuffer[i].PlayerGUID = m_playerID;
                        
                        foreach (PlayerState playerState in playerStates)
                        {
                            PlayerState player = new PlayerState(playerState);

                            m_clientStateBuffer[i].AddPlayer(player);
                        }
                        
                        m_inputBuffer[i] = new InputFrame();
                    }
                }

                public override ClientSyncState FixedUpdate()
                {
                    uint bufferIndex = m_localTick % CLIENT_STATE_BUFFER_SIZE;

                    UpdateInput(bufferIndex);

                    UpdateClientState(bufferIndex);

                    ++m_localTick;

                    ClientCorrection(m_remoteTick % CLIENT_STATE_BUFFER_SIZE);

                    for (int i = 0; i < m_updaters.Count; i++)
                    {
                        m_updaters[i].FixedUpdate(Time.deltaTime);
                    }

                    return this;
                }

                public override ClientSyncState Update()
                {
                    m_lastInput = InputController.CurrentFrame();

                    return this;
                }

                public void RegisterUpdater(IClientStateUpdater updater)
                {
                    m_updaters.Add(updater);
                }

                private void StoreCurrentStateAndStep(ref ClientState state, InputFrame input, float deltaTime)
                {
                    for (int i = 0; i < m_updaters.Count; i++)
                    {
                        m_updaters[i].SetStateAndStep(ref state, input, deltaTime);
                    }
                    
                    m_clientPhysics.Simulate(deltaTime);
                }
                
                private List<IClientStateUpdater> UpdatersNeedingCorrection(ClientState localState, ClientState remoteState)
                {
                    List<IClientStateUpdater> needCorrection = new List<IClientStateUpdater>();

                    for (int i = 0; i < m_updaters.Count; i++)
                    {
                        if (m_updaters[i].NeedsCorrection(localState, remoteState))
                        {
                            needCorrection.Add(m_updaters[i]);
                        }
                    }

                    return needCorrection;
                }

                public void ReceivePacket(UDPToolkit.Packet packet)
                {
                    // TODO remove tick from ClientSTate and add it to custom server state packet?
                    // client doesnt need its own client state ticks
                    lock (m_lock)
                    {
                        ClientState state = common.serialization.IConvertible.CreateFromBytes<ClientState>(packet.Data);
                        if (state != null)
                        {
                            state.PlayerGUID = m_playerID;
                            m_lastServerState = state;
#if DEBUG_LOG
                            Debug.Log("Received server state tick " + state.Tick.Value);
#endif //DEBUG_LOG
                            m_remoteTick = state.Tick.Value;
                        }
                    }
                }

                private void UpdateInput(uint bufferIndex)
                {
                    if (m_lastInput != null)
                    {
                        m_inputBuffer[bufferIndex].Movement.Value = m_lastInput.Movement.Value;
                        m_inputBuffer[bufferIndex].Sprinting.Value = m_lastInput.Sprinting.Value;
                    }
                    else
                    {
                        m_inputBuffer[bufferIndex].SetToNeutral();
                    }

                    m_inputBuffer[bufferIndex].Tick.Value = m_localTick;

                    m_lastInput = null;


#if NETWORK_SIMULATE
                    if (m_noServer)
                        return;
#endif // NETWORK_SIMULATE
                    // TODO: Cap max input queue size
                    // (under the hood, send multiple packets?)
                    List<common.data.InputFrame> frames = new List<common.data.InputFrame>();
                    for (uint tick = m_remoteTick; tick <= m_localTick; tick++)
                    {
                        frames.Add(m_inputBuffer[tick % CLIENT_STATE_BUFFER_SIZE]);
                    }

                    InputMessage inputMessage = new InputMessage();

                    inputMessage.PlayerID.Value = m_playerID;
                    inputMessage.StartTick.Value = m_remoteTick;
                    inputMessage.InputFrames.Value = frames;

#if NETWORK_SIMULATE
                    if (Random.Range(0f, 1f) > m_packetLossChance)
                    {
                        m_UDPClient.Send(inputMessage.GetBytes());
                    }
                    else
                    {
                        Debug.Log("SIMULATING PACKET LOSS");
                    }
#else
                    m_udpClient.Send(inputMessage.ToBytes());
#endif //NETWORK_SIMULATE       
                    
                }

                private void UpdateClientState(uint bufferIndex)
                {
                    // set current client state to last one then updating it
                    StoreCurrentStateAndStep(
                        ref m_clientStateBuffer[bufferIndex],
                        m_inputBuffer[bufferIndex],
                        Time.fixedDeltaTime);
                }

                private void ClientCorrection(uint remoteIndex)
                {
#if NETWORK_SIMULATE
                    if (m_noServer)
                        return;
#endif // NETWORK_SIMULATE
                    // receive a state from server
                    // check what tick it corresponds to
                    // rewind client state to the tick
                    // replay up to local tick by stepping every tick
                    lock (m_lock)
                    {
                        if (m_lastServerState != null)
                        {
                            List<IClientStateUpdater> updaters = UpdatersNeedingCorrection(m_clientStateBuffer[remoteIndex], m_lastServerState);
                            if (updaters.Count > 0)
                            {
                                uint rewindTicks = m_remoteTick;
                                
                                // reset world state to last server-sent state
                                for (int i = 0; i < updaters.Count; i++)
                                {
                                    updaters[i].UpdateFromState(m_lastServerState);
                                }

                                while (rewindTicks < m_localTick)
                                {
                                    uint rewindIndex = rewindTicks++ % CLIENT_STATE_BUFFER_SIZE;

                                    for (int i = 0; i < updaters.Count; i++)
                                    {
                                        updaters[i].SetStateAndStep(
                                        ref m_clientStateBuffer[rewindIndex],
                                        m_inputBuffer[rewindIndex],
                                        Time.fixedDeltaTime);
                                    }

                                    m_clientPhysics.Simulate(Time.fixedDeltaTime);
                                }
                            }
                            
                            m_lastServerState = null;
                        }
                    }
                }
            }
        }
    }
}
