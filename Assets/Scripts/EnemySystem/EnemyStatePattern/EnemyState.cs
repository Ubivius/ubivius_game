using System.Collections;
using UnityEngine;

namespace ubv.server.logic.ai
{
    abstract public class EnemyState
    {
        protected bool m_InMotion;
        // Use this for initialization
        public virtual EnemyState Init()
        {
            return this;
        }

        // Update is called once per frame
        public virtual EnemyState Update()
        {
            return this;
        }
    }
}