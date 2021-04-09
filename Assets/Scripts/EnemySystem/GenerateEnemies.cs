using System.Collections;
using ubv.common.world;
using ubv.server.logic;
using UnityEngine;

namespace Assets.Scripts.EnemySystem
{
    // MOCK CHANGE WILL BE MADE TO FIT THE PROPER REQUIREMENTS
    public class GenerateEnemies : MonoBehaviour
    {
        [SerializeField] private GameObject m_enemy;
        [SerializeField] private int m_xPos;
        [SerializeField] private int m_yPos;
        [SerializeField]  private int m_enemyCount;
        [SerializeField] private PathfindingGridManager m_pathfindingGridManager;

        private EnemyBehaviorUpdater m_enemyBehaviorUpdater;
        private PathNode[,] m_pathNodes;

        // Use this for initialization
        void Start()
        {
            m_enemyBehaviorUpdater.OnGenerateEnemy.AddListener(OnPathFindingManagerGenerated);
            m_pathfindingGridManager.OnPathFindingManagerGenerated += OnPathFindingManagerGenerated;
        }

        private void OnPathFindingManagerGenerated()
        {
            m_pathNodes = m_pathfindingGridManager.GetPathNodeArray();
            StartCoroutine(EnemySpawn());
        }

        IEnumerator EnemySpawn()
        {
            int i = 0;
            while (i < m_enemyCount)
            {
                m_xPos = Random.Range(0, m_pathNodes.GetLength(0)-1);
                m_yPos = Random.Range(0, m_pathNodes.GetLength(1)-1);

                if (m_pathfindingGridManager.GetNodeIfWalkable(m_xPos, m_yPos) != null )
                {
                    int id = System.Guid.NewGuid().GetHashCode();

                    GameObject enemy = Instantiate(m_enemy, new Vector3(m_xPos, m_yPos, 0), Quaternion.identity);
                    EnemyPathFindingMovement enemyPathFindingMovement = enemy.GetComponent<EnemyPathFindingMovement>();
                    enemyPathFindingMovement.SetManager(m_pathfindingGridManager);
                    yield return new WaitForSeconds(0.1f);
                    i++;
                }
            }
        }
    }
}