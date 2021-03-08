﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using ubv.common.world;
using UnityEngine;

namespace ubv.common.world
{
    class DoorManager
    {
        // effective range to look at ==> 4 <= x <= 6
        private const int c_lowerLookRange = 4;
        private const int c_upperLookRange = 6;

        private ubv.common.world.LogicGrid m_masterLogicGrid;
        private Tilemap m_floor;
        private Tilemap m_door;
        private TileBase m_tileFloor;
        private TileBase m_tileDoor;
        private List<RoomInfo> m_roomInMap;

        public DoorManager(dataStruct.WorldGeneratorToDoorManager worldGeneratorToDoorManager)
        {
            m_masterLogicGrid = worldGeneratorToDoorManager.masterLogicGrid;
            m_floor = worldGeneratorToDoorManager.floor;
            m_door = worldGeneratorToDoorManager.door;
            m_tileFloor = worldGeneratorToDoorManager.tileFloor;
            m_tileDoor = worldGeneratorToDoorManager.tileDoor;
            m_roomInMap = worldGeneratorToDoorManager.roomInMap;
        }

        public LogicGrid GenerateDoorGrid()
        {
            foreach (RoomInfo room in m_roomInMap)
            {
                if (!GenerateDoorForRoom(room))
                {
                    Debug.LogError("MAP CREATION ALERT : Some room does not have any door");
                }
            }
            return m_masterLogicGrid;
        }

        private bool GenerateDoorForRoom(RoomInfo room)
        {
            int numberDoor = 0;

            numberDoor += GenerateDoorNorth(room);
            numberDoor += GenerateDoorEast(room);
            numberDoor += GenerateDoorSouth(room);
            numberDoor += GenerateDoorWest(room);

            return (numberDoor == 0 ? false : true);
        }

        private int GenerateDoorNorth(RoomInfo room)
        {
            Vector2Int wallOrigin = new Vector2Int((int)room.transform.position.x, (int)room.transform.position.y + room.Height);
            List<Vector2Int> possibleDoor = new List<Vector2Int>();
            if ((int)room.transform.position.y - c_upperLookRange > 0 && (int)room.transform.position.y + room.Height + c_upperLookRange < m_masterLogicGrid.Height)
            {
                for (int i = 0 + 1; i < room.Width - 1; i++)
                {
                    if ((m_masterLogicGrid.Grid[wallOrigin.x + i - 1, wallOrigin.y + c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // 1 before
                       &&
                       (m_masterLogicGrid.Grid[wallOrigin.x + i, wallOrigin.y + c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // center
                       &&
                       (m_masterLogicGrid.Grid[wallOrigin.x + i + 1, wallOrigin.y + c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR)// 1 after
                    {
                        possibleDoor.Add(new Vector2Int(wallOrigin.x + i, wallOrigin.y));
                    }
                }
                Vector2Int doorPosition;
                if (possibleDoor.Count > 0)
                {
                    doorPosition = possibleDoor[Random.Range(0, possibleDoor.Count - 1)];
                    AddDoorNorth(doorPosition);
                    return 1;
                } 
            }
            return 0;
        }

        private void AddDoorNorth(Vector2Int doorPosition)
        {
            m_masterLogicGrid.Grid[doorPosition.x - 1, doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x,     doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x + 1, doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_door.SetTile(new Vector3Int(doorPosition.x - 1, doorPosition.y, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x,     doorPosition.y, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x + 1, doorPosition.y, 0), m_tileDoor);
            for (int i = 1; i < c_upperLookRange; i++)
            {   
                m_masterLogicGrid.Grid[doorPosition.x - 1, doorPosition.y + i] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x,     doorPosition.y + i] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x + 1, doorPosition.y + i] = new world.cellType.FloorCell();
                m_floor.SetTile(new Vector3Int(doorPosition.x - 1, doorPosition.y + i, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x,     doorPosition.y + i, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x + 1, doorPosition.y + i, 0), m_tileFloor);
            }
        }

        private int GenerateDoorEast(RoomInfo room)
        {
            Vector2Int wallOrigin = new Vector2Int((int)room.transform.position.x + room.Width, (int)room.transform.position.y);
            List<Vector2Int> possibleDoor = new List<Vector2Int>();
            if ((int)room.transform.position.x - c_upperLookRange > 0 && (int)room.transform.position.x + room.Width + c_upperLookRange < m_masterLogicGrid.Width)
            {
                for (int i = 0 + 1; i < room.Height - 1; i++)
                {
                    if ((m_masterLogicGrid.Grid[wallOrigin.x + c_upperLookRange, wallOrigin.y + i - 1])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // 1 before
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x + c_upperLookRange, wallOrigin.y + i])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // center
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x + c_upperLookRange, wallOrigin.y + i + 1])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR)// 1 after
                    {
                        possibleDoor.Add(new Vector2Int(wallOrigin.x, wallOrigin.y + i));
                    }
                }
                Vector2Int doorPosition;
                if (possibleDoor.Count > 0)
                {
                    doorPosition = possibleDoor[Random.Range(0, possibleDoor.Count - 1)];
                    AddDoorEast(doorPosition);
                    return 1;
                }
            }
            return 0;
        }

        private void AddDoorEast(Vector2Int doorPosition)
        {
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y - 1] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y    ] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y + 1] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y - 1, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y,     0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y + 1, 0), m_tileDoor);
            for (int i = 1; i < c_upperLookRange; i++)
            {
                m_masterLogicGrid.Grid[doorPosition.x + i, doorPosition.y - 1] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x + i, doorPosition.y    ] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x + i, doorPosition.y + 1] = new world.cellType.FloorCell();
                m_floor.SetTile(new Vector3Int(doorPosition.x + i, doorPosition.y - 1, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x + i, doorPosition.y,     0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x + i, doorPosition.y + 1, 0), m_tileFloor);
            }
        }

        private int GenerateDoorSouth(RoomInfo room)
        {
            Vector2Int wallOrigin = new Vector2Int((int)room.transform.position.x, (int)room.transform.position.y);
            List<Vector2Int> possibleDoor = new List<Vector2Int>();
            if ((int)room.transform.position.y - c_upperLookRange > 0 && (int)room.transform.position.y + room.Height + c_upperLookRange < m_masterLogicGrid.Height)
            {
                for (int i = 0 + 1; i < room.Width - 1; i++)
                {
                    if ((m_masterLogicGrid.Grid[wallOrigin.x + i - 1, wallOrigin.y - c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // 1 before
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x + i, wallOrigin.y - c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // center
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x + i + 1, wallOrigin.y - c_upperLookRange])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR)// 1 after
                    {
                        possibleDoor.Add(new Vector2Int(wallOrigin.x + i, wallOrigin.y - 1));
                    }
                }
                Vector2Int doorPosition;
                if (possibleDoor.Count > 0)
                {
                    doorPosition = possibleDoor[Random.Range(0, possibleDoor.Count - 1)];
                    AddDoorSouth(doorPosition);
                    return 1;
                }
            }
            return 0;
        }

        private void AddDoorSouth(Vector2Int doorPosition)
        {
            m_masterLogicGrid.Grid[doorPosition.x - 1, doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x,     doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x + 1, doorPosition.y] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_door.SetTile(new Vector3Int(doorPosition.x - 1, doorPosition.y, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x,     doorPosition.y, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x + 1, doorPosition.y, 0), m_tileDoor);
            for (int i = 1; i < c_upperLookRange; i++)
            {
                m_masterLogicGrid.Grid[doorPosition.x - 1, doorPosition.y - i] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x,     doorPosition.y - i] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x + 1, doorPosition.y - i] = new world.cellType.FloorCell();
                m_floor.SetTile(new Vector3Int(doorPosition.x - 1, doorPosition.y - i, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x,     doorPosition.y - i, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x + 1, doorPosition.y - i, 0), m_tileFloor);
            }
        }

        private int GenerateDoorWest(RoomInfo room)
        {
            Vector2Int wallOrigin = new Vector2Int((int)room.transform.position.x, (int)room.transform.position.y);
            List<Vector2Int> possibleDoor = new List<Vector2Int>();
            if ((int)room.transform.position.x - c_upperLookRange > 0 && (int)room.transform.position.x + room.Width + c_upperLookRange < m_masterLogicGrid.Width)
            {
                for (int i = 0 + 1; i < room.Height - 1; i++)
                {
                    if ((m_masterLogicGrid.Grid[wallOrigin.x - c_upperLookRange, wallOrigin.y + i - 1])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // 1 before
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x - c_upperLookRange, wallOrigin.y + i])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR // center
                        &&
                        (m_masterLogicGrid.Grid[wallOrigin.x - c_upperLookRange, wallOrigin.y + i + 1])?.GetCellType() == cellType.CellInfo.CellType.CELL_FLOOR)// 1 after
                    {
                        possibleDoor.Add(new Vector2Int(wallOrigin.x - 1, wallOrigin.y + i));
                    }
                }
                Vector2Int doorPosition;
                if (possibleDoor.Count > 0)
                {
                    doorPosition = possibleDoor[Random.Range(0, possibleDoor.Count - 1)];
                    AddDoorWest(doorPosition);
                    return 1;
                }
            }
            return 0;
        }

        private void AddDoorWest(Vector2Int doorPosition)
        {
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y - 1] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y    ] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_masterLogicGrid.Grid[doorPosition.x, doorPosition.y + 1] = new world.cellType.DoorCell(cellType.DoorType.Standard);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y - 1, 0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y,     0), m_tileDoor);
            m_door.SetTile(new Vector3Int(doorPosition.x, doorPosition.y + 1, 0), m_tileDoor);
            for (int i = 1; i < c_upperLookRange; i++)
            {
                m_masterLogicGrid.Grid[doorPosition.x - i, doorPosition.y - 1] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x - i, doorPosition.y    ] = new world.cellType.FloorCell();
                m_masterLogicGrid.Grid[doorPosition.x - i, doorPosition.y + 1] = new world.cellType.FloorCell();
                m_floor.SetTile(new Vector3Int(doorPosition.x - i, doorPosition.y - 1, 0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x - i, doorPosition.y,     0), m_tileFloor);
                m_floor.SetTile(new Vector3Int(doorPosition.x - i, doorPosition.y + 1, 0), m_tileFloor);
            }
        }


    }
}
