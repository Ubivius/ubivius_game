using UnityEngine;

/// <summary>
/// In charge of updating the state of the enemy
/// </summary>

namespace ubv.server.logic.ai
{
    public class EnemyStateMachine : MonoBehaviour
    {
        private EnemyState m_currentEnemyState;
        //public Transform player;
        private EnemyPathFindingMovement m_pathfindingMovement;

        private void Awake()
        {
            m_pathfindingMovement = GetComponent<EnemyPathFindingMovement>();
        }

        // Use this for initialization
        void Start()
        {
           m_currentEnemyState = new RoamingState(m_pathfindingMovement);
        }

        // Update is called once per frame
        void Update()
        {
            m_currentEnemyState = m_currentEnemyState.Update();
        }
    }
}