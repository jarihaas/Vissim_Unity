using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vissim.Route
{
    public class Highlighted
    {
        public readonly int Selected_Route;

        public readonly int Selected_Routing_Decision;

        public readonly List<Logic.Link_Segment> Highlighted_Link_Segments;

        public Highlighted  (   int                         Selected_Route,
                                int                         Selected_Routing_Decision,
                                List<Logic.Link_Segment>    Highlighted_Link_Segments)
        {
            this.Selected_Route             = Selected_Route;
            this.Selected_Routing_Decision  = Selected_Routing_Decision;
            this.Highlighted_Link_Segments  = Highlighted_Link_Segments;
        }
    }

}
