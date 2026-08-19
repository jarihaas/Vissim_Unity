using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Vissim.Signal
{
    public class Head
    {

        // signal SelectedRoute
        public long No
        { get; set; }

        // link obj
        // public Link Link { get; set; }

        // lane SelectedRoute
        public Logic.Lane Lane
        { get; set; }

        // position of signal
        public float Pos
        { get; set; }

        // signal controller SelectedRoute
        public long Controller
        { get; set; }

        // signal group SelectedRoute
        public long SG
        { get; set; }

        // Color of the signal
        public Color Colour
        { get; set; }

        // Unity game object
        public GameObject Obj
        { get; set; }
        
        // method for update the signal state and color
        public void UpdateState()
        {
            MeshRenderer Mesh_Renderer = Obj.GetComponent<MeshRenderer>();  
            Mesh_Renderer.material.color = Colour;
        }
    }
}