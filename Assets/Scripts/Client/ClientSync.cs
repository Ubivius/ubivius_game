using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;

namespace ubv
{
    namespace client
    {
        /// <summary>
        /// Client-side synchronisation with server information
        /// </summary>
        public class ClientSync : MonoBehaviour
        {
            // TO CHECK:: https://www.codeproject.com/Articles/311944/BinaryFormatter-or-Manual-serializing
            // https://github.com/spectre1989/unity_physics_csp/blob/master/Assets/Logic.cs

            client.logic.ClientSyncState m_currentState;

#if NETWORK_SIMULATE
            [SerializeField] private float m_packetLossChance = 0.15f;
#endif // NETWORK_SIMULATE
            
            [SerializeField] private udp.client.UDPClient m_udpClient;

            [SerializeField] private InputController m_inputController;
            [SerializeField] private string m_physicsScene;

            private void Awake()
            {

            }

            private void Start()
            {
                m_currentState = new logic.ClientSyncInit(m_udpClient, m_physicsScene, m_inputController);
            }

            private void Update()
            {
                m_currentState = m_currentState.Update();
            }

            private void FixedUpdate()
            {
                m_currentState = m_currentState.FixedUpdate();
            }
        }
    }
}
