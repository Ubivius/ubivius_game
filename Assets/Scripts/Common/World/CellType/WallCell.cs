﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ubv.common.world.cellType
{
    class WallCell : LogicCell
    {
        public WallCell()
        {
            IsWalkable = false;
        }
    }
}
