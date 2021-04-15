using System;
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
            CELL_FLOOR,
            CELL_PLAYERSPAWN,
            CELL_NONE
        }

        private serialization.types.Byte m_cellType;
        private serialization.types.ByteArray m_logicCellBytes;

        public CellInfo() : base()
        {
            m_cellType = new serialization.types.Byte((byte)CellType.CELL_NONE);
            m_logicCellBytes = new serialization.types.ByteArray(new byte[0]);

            InitSerializableMembers(m_cellType, m_logicCellBytes);
        }

        public CellInfo(LogicCell parentCell)
        {
            if (parentCell != null)
            {
                m_cellType = new serialization.types.Byte((byte)parentCell.GetCellType());
                m_logicCellBytes = new serialization.types.ByteArray(parentCell.GetBytes());
            }
            else
            {
                m_cellType = new serialization.types.Byte((byte)CellType.CELL_NONE);
                m_logicCellBytes = new serialization.types.ByteArray(new byte[0]);
            }

            InitSerializableMembers(m_cellType, m_logicCellBytes);
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
                    cell = CreateFromBytes<WallCell>(m_logicCellBytes.Value.ArraySegment());
                    break;
                case CellType.CELL_FLOOR:
                    cell = CreateFromBytes<FloorCell>(m_logicCellBytes.Value.ArraySegment());
                    break;
                case CellType.CELL_DOOR:
                    cell = CreateFromBytes<DoorCell>(m_logicCellBytes.Value.ArraySegment());
                    break;
                case CellType.CELL_BUTTON:
                    cell = CreateFromBytes<DoorButtonCell>(m_logicCellBytes.Value.ArraySegment());
                    break;
                case CellType.CELL_PLAYERSPAWN:
                    cell = CreateFromBytes<PlayerSpawnCell>(m_logicCellBytes.Value.ArraySegment());
                    break;
                case CellType.CELL_NONE:
                    break;
                default:
                    break;
            }
            return cell;
        }
    }

    abstract public class LogicCell : serialization.Serializable
    {
        public delegate void LogicCellDelegate(LogicCell Cell);
        public LogicCellDelegate OnChange;

        private serialization.types.Bool m_isWalkable;
        private serialization.types.Int32 m_cellID;

        static private int m_cellsCreated = 0;

        public LogicCell()
        {
            m_cellID = new serialization.types.Int32(m_cellsCreated++);
            m_isWalkable = new serialization.types.Bool(false);

            InitSerializableMembers(m_cellID, m_isWalkable);
        }
        
        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.LOGIC_CELL;
        }

        public int GetCellID()
        {
            return m_cellID.Value;
        }

        public abstract CellInfo.CellType GetCellType();

        public bool IsWalkable { get => m_isWalkable.Value; protected set => SetWalkable(value); }

        private void SetWalkable(bool value)
        {
            m_isWalkable.Value = value;
            OnChange?.Invoke(this);
        }
    }
}
