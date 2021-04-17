﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ubv.common.world
{
    public class RoomInfo : MonoBehaviour
    {
        [SerializeField] private string m_name;
        [SerializeField] private Tilemap m_floor;
        [SerializeField] private Tilemap m_wall;
        [SerializeField] private Tilemap m_door;
        [SerializeField] private Tilemap m_interactable;
        [SerializeField] private Tilemap m_finalDoor;
        [SerializeField] private Tilemap m_sectionDoorButton;
        [SerializeField] private Tilemap m_sectionButton;
        [SerializeField] private Tilemap m_finalButton;
        [SerializeField] private Tilemap m_playerSpawnZone;

        [SerializeField] private int m_height;
        [SerializeField] private int m_width;
        private Vector3Int m_roomOrigin;

        private LogicGrid m_logicGrid;

        public int Height { get => m_height; private set => m_height = value; }
        public int Width { get => m_width; private set => m_width = value; }
        public LogicGrid LogicGrid { get => m_logicGrid; private set => m_logicGrid = value; }

        private List<cellType.Section> m_listSection = new List<cellType.Section>() 
        { 
            cellType.Section.NorthEast, 
            cellType.Section.NorthWest, 
            cellType.Section.SouthEast, 
            cellType.Section.SouthWest 
        }; 

        private void Awake()
        {
#if DEBUG
            Debug.Assert(m_floor != null);
#endif // DEBUG

            RoomManagement();

            LogicGrid = new LogicGrid(Width, Height);

            DoorManagement();
            InteractableManagement();
            WallManagement();
            PlayerSpawnManagement();
            FloorManagement();


            FinalDoorManagement();
            FinalButtonManagement();
        }
        private void RoomManagement()
        {
            m_floor.CompressBounds();
            Width = m_floor.cellBounds.size.x;
            Height = m_floor.cellBounds.size.y;
            m_roomOrigin = m_floor.origin;
        }
        private void FloorManagement()
        {
            m_floor.CompressBounds();
            Vector3Int iterateur = new Vector3Int();
            for (iterateur.x = m_roomOrigin.x; iterateur.x < m_floor.cellBounds.size.x + m_roomOrigin.x; iterateur.x++)
            {
                for (iterateur.y = m_roomOrigin.y; iterateur.y < m_floor.cellBounds.size.y + m_roomOrigin.y; iterateur.y++)
                {
                    if (m_floor.HasTile(iterateur))
                    {
                        if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                        {
                            LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.FloorCell();
                        }
                    }
                }
            }
        }
        private void WallManagement()
        {
            if (m_wall)
            {
                m_wall.CompressBounds();
                Vector3Int originOffset = m_wall.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_wall.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_wall.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_wall.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x,iterateur.y] = new cellType.WallCell();
                            }                            
                        }
                    }
                }
            }
        }
        private void DoorManagement()
        {
            if (m_door)
            {
                m_door.CompressBounds();
                Vector3Int originOffset = m_door.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_door.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_door.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_door.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.DoorCell(cellType.DoorType.Standard);
                            }                        
                        }
                    }
                }
            }
        }
        private void InteractableManagement()
        {
            if (m_interactable)
            {
                m_interactable.CompressBounds();
                Vector3Int originOffset = m_interactable.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_interactable.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_interactable.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_interactable.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.DoorButtonCell();
                            }
                        }
                    }
                }
            }
        }

        private void PlayerSpawnManagement()
        {
            if (m_playerSpawnZone)
            {
                m_playerSpawnZone.CompressBounds();
                Vector3Int originOffset = m_playerSpawnZone.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_playerSpawnZone.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_playerSpawnZone.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_playerSpawnZone.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.PlayerSpawnCell();
                            }
                        }
                    }
                }
            }
        }

        private void SectionDoorButtonManagement() // m_listSection //  Random.Range(0, dir.Count - 1); // dir.RemoveAt(select);
        {
            if (m_sectionDoorButton)
            {
                m_sectionDoorButton.CompressBounds();
                Vector3Int originOffset = m_sectionDoorButton.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_sectionDoorButton.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_sectionDoorButton.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_sectionDoorButton.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                int select = Random.Range(0, m_listSection.Count - 1);
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.SectionButton(m_listSection[select]);
                                m_listSection.RemoveAt(select);
                            }
                        }
                    }
                }
            }
        }

        private void SectionButtonManagement()
        {
            if (m_sectionButton)
            {
                m_sectionButton.CompressBounds();
                Vector3Int originOffset = m_sectionButton.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_sectionButton.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_sectionButton.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_sectionButton.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                int select = Random.Range(0, m_listSection.Count - 1);
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.SectionButton(m_listSection[select]);
                                m_listSection.RemoveAt(select);
                            }
                        }
                    }
                }
            }
        }

        private void FinalButtonManagement()
        {
            if (m_finalButton)
            {
                m_finalButton.CompressBounds();
                Vector3Int originOffset = m_finalButton.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_finalButton.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_finalButton.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_finalButton.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.FinalButtonCell();
                            }
                        }
                    }
                }
            }
        }

        private void FinalDoorManagement()
        {
            if (m_finalDoor)
            {
                m_finalDoor.CompressBounds();
                Vector3Int originOffset = m_finalDoor.origin - m_roomOrigin;
                Vector3Int iterateur = new Vector3Int();
                for (iterateur.x = originOffset.x; iterateur.x < m_finalDoor.cellBounds.size.x + originOffset.x; iterateur.x++)
                {
                    for (iterateur.y = originOffset.y; iterateur.y < m_finalDoor.cellBounds.size.y + originOffset.y; iterateur.y++)
                    {
                        if (m_finalDoor.HasTile(iterateur))
                        {
                            if (LogicGrid.Grid[iterateur.x, iterateur.y] == null)
                            {
                                LogicGrid.Grid[iterateur.x, iterateur.y] = new cellType.DoorCell(cellType.DoorType.FinalDoor);
                            }
                        }
                    }
                }
            }
        }

        Vector2Int GetDimension()
        {
            return new Vector2Int(Width, Height);
        }

    }

}
