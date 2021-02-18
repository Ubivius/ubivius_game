﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ubv.common.serialization;

namespace ubv.common.world.cellType
{
    public class CellInfo : serialization.Serializable
    {
        public enum CellType
        {
            CELL_WALL,
            CELL_DOOR,
            CELL_BUTTON,
            CELL_FLOOR
        }

        private serialization.types.Byte m_cellType;
        private serialization.types.Int32 m_cellID;
        private serialization.types.ByteArray m_logicCellBytes;

        public CellInfo() : base()
        {
            m_cellType = new serialization.types.Byte((byte)0);
            m_cellID = new serialization.types.Int32(-1);
            m_logicCellBytes = new serialization.types.ByteArray(null);

            InitSerializableMembers(m_cellType, m_cellID, m_logicCellBytes);
        }

        public CellInfo(LogicCell parentCell)
        {
            CellType cellType = parentCell.GetCellType();
            int cellID = parentCell.GetCellID();
            m_cellType = new serialization.types.Byte((byte)cellType);
            m_cellID = new serialization.types.Int32(cellID);
            m_logicCellBytes = new serialization.types.ByteArray(parentCell.GetBytes());

            InitSerializableMembers(m_cellType, m_cellID, m_logicCellBytes);
        }

        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.LOGIC_CELL_INFO;
        }
        
        public LogicCell CellFromBytes()
        {
            LogicCell cell = null;
            switch((CellType)m_cellType.Value)
            {
                case CellType.CELL_WALL:
                    cell = CreateFromBytes<WallCell>(m_logicCellBytes.Value);
                    break;
                case CellType.CELL_FLOOR:
                    cell = CreateFromBytes<FloorCell>(m_logicCellBytes.Value);
                    break;
                case CellType.CELL_DOOR:
                    cell = CreateFromBytes<DoorCell>(m_logicCellBytes.Value);
                    break;
                case CellType.CELL_BUTTON:
                    cell = CreateFromBytes<DoorButtonCell>(m_logicCellBytes.Value);
                    break;
                default:
                    break;
            }

            return cell;
        }
    }

    abstract public class LogicCell : serialization.Serializable
    {
        private serialization.types.Bool m_isWalkable;
        private int m_cellID;

        public LogicCell()
        {
            m_cellID = System.Guid.NewGuid().GetHashCode();
            m_isWalkable = new serialization.types.Bool(false);

            InitSerializableMembers(m_isWalkable);
        }
        
        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.LOGIC_CELL;
        }

        public int GetCellID()
        {
            return m_cellID;
        }

        public abstract CellInfo.CellType GetCellType();

        public bool IsWalkable { get => m_isWalkable.Value; protected set => m_isWalkable.Value = value; }
    }
}
