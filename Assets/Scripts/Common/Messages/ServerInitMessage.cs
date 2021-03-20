﻿using UnityEngine;
using System.Collections;
using ubv.common.serialization;
using System.Collections.Generic;

namespace ubv.common.data
{ 
    public class PlayerStateList : serialization.types.List<PlayerState>
    {
        public PlayerStateList(List<PlayerState> players) : base(players)
        { }

        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.LIST_PLAYERSTATE;
        }
    }

    public class ServerInitMessage : serialization.Serializable
    {

        public serialization.types.Int32 SimulationBuffer { get; protected set; }
        public PlayerStateList Players { get; protected set; }
        public world.LogicGrid.CellInfo2DArray CellInfo2DArray;

        public ServerInitMessage()
        {
            SimulationBuffer = new serialization.types.Int32(0);
            Players = new PlayerStateList(null);
            CellInfo2DArray = new world.LogicGrid.CellInfo2DArray(null);

            InitSerializableMembers(SimulationBuffer, Players, CellInfo2DArray);
        }

        public ServerInitMessage(int simulationBuffer, List<PlayerState> players, world.cellType.CellInfo[,] array) : base()
        {
            SimulationBuffer = new serialization.types.Int32(simulationBuffer);
            Players = new PlayerStateList(players);
            CellInfo2DArray = new world.LogicGrid.CellInfo2DArray(array);

            InitSerializableMembers(SimulationBuffer, Players, CellInfo2DArray);
        }
                
        protected override ID.BYTE_TYPE SerializationID()
        {
            return ID.BYTE_TYPE.SERVER_INIT_MESSAGE;
        }
    }
}
