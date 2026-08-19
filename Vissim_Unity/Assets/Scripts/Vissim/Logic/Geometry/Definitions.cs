using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Vissim.Logic.Geometry
{
    struct Lane_Segment_Coordinates
    {
        public Vector3 Mid_Pos;
        public Vector3 Vector_Along_Segment;
        public Vector3 Min_Coord;
        public Vector3 Max_Coord;
    }
}
