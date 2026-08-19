using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Vissim.Route
{
    public class Vehicle_Route
    {
        public int Dest_Link { get; set; }
        public double Dest_Position_On_Link { get; set; }

        public List<int> List_Of_Passed_Links;
    }
}
