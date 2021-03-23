﻿using System.Collections;
using UnityEngine;

namespace ubv.server.logic.ai
{
    public class RoamingState : EnemyState
    {
        private EnemyPathFindingMovement m_enemyPathFindingMovement;
        private Vector2 m_startingPosition;
        private Vector2 m_roamPosition;

        private Transform m_player;

        public RoamingState(EnemyPathFindingMovement enemyPathFindingMovement, Transform player)
        {
            m_enemyPathFindingMovement = enemyPathFindingMovement;

            m_startingPosition = m_enemyPathFindingMovement.GetPosition();
            m_roamPosition = GetRoamingPosition();
            m_player = player;
        }

        // Update is called once per frame
        public EnemyState Update()
        {
            m_enemyPathFindingMovement.MoveToTimer(m_roamPosition);

            float reachedPositionDistance = 10f;
            if (Vector3.Distance(m_enemyPathFindingMovement.GetPosition(), m_roamPosition) < reachedPositionDistance)
            {
                // Reached Roam Position
                m_roamPosition = GetRoamingPosition();
            }

            return FindTarget();
        }

        private Vector3 GetRoamingPosition()
        {
            return m_startingPosition + GetRandomDir() * Random.Range(10f, 70f);
        }

        private EnemyState FindTarget()
        {
            float targetRange = 50f;
            if (Vector3.Distance(m_enemyPathFindingMovement.GetPosition(), m_player.position) < targetRange)
            {
                // Player within target range
                //return new ChasingTargetState();
            }

            return this;
        }

        /// METTRE ÇA DANS UN FICHIER UTILS PLUS TARD AVEC PLUSIEURS AUTRES FONCTIONS STATIQUES???
        public static Vector2 GetRandomDir()
        {
            return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
    }
}
