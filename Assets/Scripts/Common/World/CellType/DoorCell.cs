﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ubv.common.serialization;

namespace ubv.common.world.cellType
{
    public enum DoorType
    {
        Standard,
        Section
    }

    public class DoorCell : LogicCell
    {
        private serialization.types.Int32 m_doorType;
        private serialization.types.Bool m_IsClosed;
        private serialization.types.Int32 m_cellID;
        
        public DoorCell(DoorType doorType) : base()
        {
            IsWalkable = true;
            DoorType = doorType;

            m_IsClosed = new serialization.types.Bool(false);
            m_doorType = new serialization.types.Int32((int)DoorType.Standard);
            m_cellID = new serialization.types.Int32(System.Guid.NewGuid().GetHashCode());
        }

        public void CloseDoor()
        {
            m_IsClosed.Value = true;
            IsWalkable = false;
        }

        public void OpenDoor()
        {
            m_IsClosed.Value = (false);
            IsWalkable = true;
        }

        public DoorType DoorType { get => (DoorType)m_doorType.Value; private set => m_doorType.Value = (int)value; }
        public int CellID { get => m_cellID.Value; private set => m_cellID.Value = value; }

        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.LOGIC_CELL_DOOR;
        }
    }
}
