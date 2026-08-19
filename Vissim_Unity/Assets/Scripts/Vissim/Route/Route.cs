using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vissim.Route
{
    public class Route
    {
        public int Dest_Link
        { get; set; }
        
        public Vector3 Dest_Position
        { get; set; }

        public List<Point> Route_Points;

        public Route()
        {
            Route_Points = new List<Point>();
        }
    }
}
