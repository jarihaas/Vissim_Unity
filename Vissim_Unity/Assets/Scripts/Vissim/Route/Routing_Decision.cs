using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vissim.Route
{
    public class Routing_Decision
    {
        public Dictionary<int, Route> Routes;

        public  int Start_Link
        { get; set; }
        
        public Vector3 Pos
        { get; set; }

        public Routing_Decision()
        {
            Routes = new Dictionary<int, Route>();
        }
    }
}