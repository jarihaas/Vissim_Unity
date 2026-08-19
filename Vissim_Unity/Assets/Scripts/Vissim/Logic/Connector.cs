using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Vissim.Logic
{
    public class Connector : Link
    {
        public int From_Link_No
        { get; set; }

        public int From_Lane_No
        { get; set; }
        
        public  double From_Pos
        { get; set; }
        
        public int To_Link_No
        { get; set; }
        
        public int To_Lane_No
        { get; set; }
        
        public double To_Pos
        { get; set; }
        
        public int Lane_Count
        { get; set; }
    }
}
