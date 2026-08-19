using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Vissim.Logic
{
    public class Link_Segment 
    {

        public  int ID
        { get; set; }

        public Vector3 Start
        { get; set; }
        
        public Vector3 End
        { get; set; }

        public List<GameObject> Lane_GameObj_Of_Link_Segment = new List<GameObject>();

        public List<Route.Reference> Routes_Passing_Over;

        // calc length of the lane in 3D
        public float X_Kat 
        { get { return this.End.x - this.Start.x; } }

        public float Z_Kat
        { get { return this.End.z - this.Start.z; } }

        public float XZ_Hyp
        { get { return Mathf.Sqrt(Mathf.Pow(this.X_Kat, 2) + Mathf.Pow(this.Z_Kat, 2)); } }

        public float Y_Kat
        { get { return this.End.y - this.Start.y; } }

        public float XYZ_Kat
        { get { return Mathf.Sqrt(Mathf.Pow(this.XZ_Hyp, 2) + Mathf.Pow(this.Y_Kat, 2)); } }

        public Link_Segment()
        { Routes_Passing_Over = new List<Route.Reference>(); }
    }
}