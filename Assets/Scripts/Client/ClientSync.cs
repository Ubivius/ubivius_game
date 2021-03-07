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
            

#if NETWORK_SIMULATE
            [HideInInspector] public UnityEngine.Events.UnityEvent ConnectButtonEvent;
#endif // NETWORK_SIMULATE
            
            

#if NETWORK_SIMULATE
            public void InvokeConnect()
            {
                ConnectButtonEvent.Invoke();
            }
#endif // NETWORK_SIMULATE
        }
    }
}
