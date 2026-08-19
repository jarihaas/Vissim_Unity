using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vissim.Route
{
    public enum Point_Type
    {
        Decision,
        SegmentStart,
        SegmentEnd,
        EnterLink,
        LeaveLink
    }

    public class Point
    {
        public int Link;

        public Vector3 Pos;

        public Point_Type Type;
    }
}
