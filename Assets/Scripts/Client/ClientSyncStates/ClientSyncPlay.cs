﻿using UnityEngine;
using System.Collections;
using System.Net;
using ubv.udp;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ubv.common.data;
using ubv.tcp;

namespace ubv.client.logic
{
    /// <summary>
    /// Represents the state of the server during the game
    /// </summary>
    public class ClientSyncPlay : ClientSyncState, udp.client.IUDPClientReceiver
    {
        [SerializeField] private PlayerSettings m_playerSettings;
        [SerializeField] private string m_physicsScene;
        private int m_playerID;

        private int m_simulationBuffer;
        private uint m_remoteTick;
        private uint m_localTick;

        private const ushort CLIENT_STATE_BUFFER_SIZE = 64;

        private ClientState[] m_clientStateBuffer;
        private InputFrame[] m_inputBuffer;
        private InputFrame m_lastInput;
        private ClientState m_lastServerState;

        private PhysicsScene2D m_clientPhysics;

        private List<IClientStateUpdater> m_updaters;

        private bool m_initialized;

#if NETWORK_SIMULATE
        [SerializeField] private float m_packetLossChance = 0.15f;
        [SerializeField] private bool m_noServer;
#endif // NETWORK_SIMULATE

        protected override void StateAwake()
        {
            ClientSyncState.PlayState = this;
        }

        public void Init(int playerID, int simulationBuffer, List<PlayerState> playerStates)
        {
            m_localTick = 0;
            m_clientStateBuffer = new ClientState[CLIENT_STATE_BUFFER_SIZE];
            m_inputBuffer = new InputFrame[CLIENT_STATE_BUFFER_SIZE];

            m_clientPhysics = SceneManager.GetSceneByName(m_physicsScene).GetPhysicsScene2D();
            m_lastServerState = null;

            m_updaters = new List<IClientStateUpdater>();
            m_initialized = false;

            m_playerID = playerID;
            m_simulationBuffer = simulationBuffer;
            Dictionary<int, PlayerState> playerStateDict = new Dictionary<int, PlayerState>();
            foreach (PlayerState state in playerStates)
            {
                playerStateDict[state.GUID.Value] = state;
            }

            m_updaters.Add(new PlayerGameObjectUpdater(m_playerSettings, playerStateDict, m_playerID));

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
            m_initialized = true;
        }

        protected override void StateFixedUpdate()
        {
            if (!m_initialized)
                return;

            uint bufferIndex = m_localTick % CLIENT_STATE_BUFFER_SIZE;

            UpdateInput(bufferIndex);

            UpdateClientState(bufferIndex);

            ++m_localTick;

            ClientCorrection(m_remoteTick % CLIENT_STATE_BUFFER_SIZE);

            for (int i = 0; i < m_updaters.Count; i++)
            {
                m_updaters[i].FixedUpdate(Time.deltaTime);
            }

            //return this;
        }

        protected override void StateUpdate()
        {
            if (!m_initialized)
                return;

            m_lastInput = InputController.CurrentFrame();
        }

        public void RegisterUpdater(IClientStateUpdater updater)
        {
            if (!m_initialized)
                return;

            m_updaters.Add(updater);
        }

        private void StoreCurrentStateAndStep(ref ClientState state, InputFrame input, float deltaTime)
        {
            if (!m_initialized)
                return;

            for (int i = 0; i < m_updaters.Count; i++)
            {
                m_updaters[i].SetStateAndStep(ref state, input, deltaTime);
            }
                    
            m_clientPhysics.Simulate(deltaTime);
        }
                
        private List<IClientStateUpdater> UpdatersNeedingCorrection(ClientState localState, ClientState remoteState)
        {
            if (!m_initialized)
                return null;

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
            if (!m_initialized)
                return;

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

                    if(m_localTick < m_remoteTick)
                    {
#if DEBUG_LOG
                        Debug.Log("Client has fallen behind by " + (m_remoteTick - m_localTick) + ". Fast-forwarding.");
#endif //DEBUG_LOG
                        m_localTick = m_remoteTick + (uint)m_simulationBuffer;
                    }

                    // PATCH FOR JITTER (too many phy simulate calls)
                    // TODO: investigate (si le temps le permet)
                    // ClientCorrection()
                    if(m_localTick > m_remoteTick + (uint)m_simulationBuffer)
                    {
                        m_localTick = m_remoteTick ;
                    }
                }
            }
        }

        private void UpdateInput(uint bufferIndex)
        {
            if (!m_initialized)
                return;

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

            List<common.data.InputFrame> frames = new List<common.data.InputFrame>();
            for (uint tick = (uint)Mathf.Max((int)m_remoteTick, (int)m_localTick - (m_simulationBuffer * 2)); tick <= m_localTick; tick++)
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
            if (!m_initialized)
                return;
            // set current client state to last one then updating it
            StoreCurrentStateAndStep(
                ref m_clientStateBuffer[bufferIndex],
                m_inputBuffer[bufferIndex],
                Time.fixedDeltaTime);
        }

        private void ClientCorrection(uint remoteIndex)
        {
            if (!m_initialized)
                return;

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