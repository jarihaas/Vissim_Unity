using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Vissim.Interface
{
    public partial class Inpx_Importer
    {
        /*****************************/
        /* Predicates and properties */
        /*****************************/
        public Vector3 Starting_Pos { get; set; }
        
        public Vector3 Starting_Orientation { get; set; }

        private string File_Name { get; set; }
        private float Ground_Z_Offset { get; set; }
        private float Line_Width { get; set; }
        private float Lane_Height { get; set; }
        private bool Fill_Seg_Gaps { get; set; }

        private Dictionary<int, float> Level_Z { get; set; }
        private Dictionary<int, bool> Display_Type_Visibilities { get; set; }
        public Dictionary<long, Signal.Group> Signal_Controllers = new Dictionary<long, Signal.Group>();
        private Dictionary<long, Signal.Head> Signal_Heads { get; set; }
        private XmlDocument XML { get; set; }

        /* Collection of links */
        public Dictionary<long, Logic.Link> Links { get; private set; }
        private float X_Max { get; set; }
        private float Z_Max { get; set; }
        private float X_Min { get; set; }
        private float Z_Min { get; set; }
        private float Y_Min { get; set; }

        /* Reading Routes */
        public Dictionary<int, Route.Routing_Decision> Routing_Decision_Dict { get; private set; }
        public Logic.Geometry.Relative_To_World Relative_To_World = new Logic.Geometry.Relative_To_World();

        /* Simulation steps in ticks */
        public long Sim_Steps_In_Ticks { get; private set; }

        /***********/
        /* Methods */
        /***********/
        public Inpx_Importer(string File_Name,
                                float Ground_Z_Offset,
                                float Line_Width,
                                float Lane_Height,
                                bool Fill_Seg_Gaps)
        {
            this.File_Name = File_Name;

            this.Ground_Z_Offset = Ground_Z_Offset;
            this.Line_Width = Line_Width;
            this.Lane_Height = Lane_Height;
            this.Fill_Seg_Gaps = Fill_Seg_Gaps;

            /* Note: Since the original author set this predicates as
                get/set,  I believe this values are meant to change. 
                Otherwise, it would be extremely stupid to make 
                constants as get/set. */
            this.X_Max = float.MinValue;
            this.Z_Max = float.MinValue;
            this.X_Min = float.MaxValue;
            this.Z_Min = float.MaxValue;
            this.Y_Min = float.MaxValue;
        }
    }
}