using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Vissim.Logic
{
    public class Link
    {
        // SelectedRoute
        public long ID
        { get; set; }

        // all segments, which were needed for the link
        public List<Link_Segment> Link_Segments
        { get; set; }

        // all lanes on link
        public Dictionary<long, Lane> Lanes
        { get; set; }

        public int Level
        { get; set; }
        
        public bool Visible
        { get; set; }

        // all signals on this link
        public Dictionary<long, Signal.Head> Signals
        { get; set; }

        public Link() 
        {
            Link_Segments = new List<Link_Segment>();
            Lanes = new Dictionary<long, Lane>();
        }

        // returns a list with all lane widths
        public List<float> Get_Lane_Widths()
        {
            List<float> Widths = new List<float>();

            foreach (Lane lane in Lanes.Values)
            {
                Widths.Add(lane.Width);
            }

            return Widths;
        }
    }
}