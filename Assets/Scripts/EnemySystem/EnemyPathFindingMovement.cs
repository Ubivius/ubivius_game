using System;
using System.Collections;
using System.Collections.Generic;
using ubv.server.logic;
using UnityEngine;

/*
 * Responsible for all Enemy Movement Pathfinding
 * */
public class EnemyPathFindingMovement : MonoBehaviour
{
    [SerializeField] private const float SPEED = 5f;
    [SerializeField] private Transform m_player;

    private PathfindingGridManager m_pathfindingGridManager;
    private EnemyMain m_enemyMain;
    private List<Vector2> m_pathVectorList;
    private int m_currentPathIndex;
    private Vector2 m_moveDir;
    private Vector2 m_lastMoveDir;
    private float reachedTargetDistance = 0.01f;

    private void Awake()
    {
        m_enemyMain = GetComponent<EnemyMain>();
    }

    private void Update()
    {
        HandleMovement();
        m_enemyMain.EnemyRigidbody2D.velocity = m_moveDir * SPEED;
    }

    private void HandleMovement()
    {
        PrintPathfindingPath();
        if (m_pathVectorList != null && m_pathVectorList.Count > 0)
        {
            Vector2 targetPosition = m_pathVectorList[m_currentPathIndex];
            if (Vector2.SqrMagnitude(GetPosition() - targetPosition) > reachedTargetDistance)
            {
                m_moveDir = (targetPosition - GetPosition()).normalized;
                m_lastMoveDir = m_moveDir;
                //future moving animation
                //m_enemyMain.CharacterAnims.PlayMoveAnim(moveDir);
            }
            else
            {
                m_currentPathIndex++;
                if (m_currentPathIndex >= m_pathVectorList.Count)
                {
                    StopMoving();
                    //m_enemyMain.CharacterAnims.PlayIdleAnim();
                }
            }
        }
        else
        {
            //m_enemyMain.CharacterAnims.PlayIdleAnim();
        }
    }

    public void SetManager(PathfindingGridManager pathfindingGridManager)
    {
        m_pathfindingGridManager = pathfindingGridManager;
    }

    public void StopMoving()
    {
        m_pathVectorList = null;
        m_moveDir = Vector2.zero;
    }

    public List<Vector2> GetPathVectorList()
    {
        return m_pathVectorList;
    }

    private void PrintPathfindingPath()
    {
        if (m_pathVectorList != null)
        {
            for (int i = 0; i < m_pathVectorList.Count - 1; i++)
            {
                Debug.DrawLine(m_pathVectorList[i], m_pathVectorList[i + 1]);
            }
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        if (targetPosition != null)
        {
            SetTargetPosition(targetPosition);
        }
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        if (m_pathfindingGridManager != null && m_pathfindingGridManager.IsSetUpDone() == true)
        {
            m_currentPathIndex = 0;
            m_pathVectorList = m_pathfindingGridManager.GetPathRoute(GetPosition(), targetPosition).PathVectorList;
        }
    }

    public Vector2 GetPosition()
    {
        return (Vector2) transform.position;
    }

    public Vector2 GetPlayerPosition()
    {
        return (Vector2) m_player.position;
    }

    public Vector2 GetLastMoveDir()
    {
        return m_lastMoveDir;
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
        m_enemyMain.EnemyRigidbody2D.velocity = Vector2.zero;
    }

    public bool IsPositionWalkable(Vector2 worldPosition)
    {
        if (m_pathfindingGridManager.GetNodeIfWalkable(worldPosition.x, worldPosition.y) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
