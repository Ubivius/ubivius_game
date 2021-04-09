using System.Collections;
using UnityEngine;

namespace ubv.server.logic.ai
{
    public class RoamingState : EnemyState
    {
        private EnemyPathFindingMovement m_enemyPathFindingMovement;
        private float m_reachedPositionDistance = 1f;
        private float m_targetRange = 50f;
        private Vector2 m_startingPosition;
        private Vector2 m_roamPosition;

        public RoamingState(EnemyPathFindingMovement enemyPathFindingMovement)
        {
            m_enemyPathFindingMovement = enemyPathFindingMovement;

            m_InMotion = false;
            m_startingPosition = m_enemyPathFindingMovement.GetPosition();
            m_roamPosition = GetRoamingPosition();
        }

        // Update is called once per frame
        public override EnemyState Update()
        {
            if (!m_enemyPathFindingMovement.IsPositionWalkable(m_roamPosition)|| Vector3.SqrMagnitude(m_roamPosition - m_enemyPathFindingMovement.GetPosition()) < m_reachedPositionDistance * m_reachedPositionDistance /*|| verify if target is walkable with a getnode*/)
            {
                // Reached Roam Position
                m_roamPosition = GetRoamingPosition();

                m_InMotion = false;
            }
            
            else if(!m_InMotion)
            {
                m_enemyPathFindingMovement.MoveTo(m_roamPosition);
                m_InMotion = true;
            }

            return FindTarget();
        }

        private Vector3 GetRoamingPosition()
        {
            return m_startingPosition + Utils.GetRandomDir() * Random.Range(1f, 10f);
        }

        private EnemyState FindTarget()
        {

            //if (Vector3.SqrMagnitude(m_enemyPathFindingMovement.GetPosition() - m_enemyPathFindingMovement.GetPlayerPosition()) < m_targetRange * m_targetRange)
            //{
            // Player within target range
            //return new ChasingTargetState();
            //}

            return this;
        }
    }
}
