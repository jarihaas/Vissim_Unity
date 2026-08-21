using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using Vissim.Logic;

namespace Vissim.Interface
{
    public partial class Inpx_Importer
    {
        private void Spawn_Links()
        {
            foreach (Logic.Link Link in Links.Values)
            {
                if (Link.Visible)
                    Spawn_Link(Link);
            }
        }

        private void Spawn_Link(Logic.Link Link)
        {
            foreach (Logic.Link_Segment Seg in Link.Link_Segments)
                Spawn_Link_Seg(Seg, Link);
        }

        private void Spawn_Ground()
        {
            GameObject Ground = Logic.Spawner.Spawn("Ground");

            /* Scale */
            Ground.transform.localScale = new Vector3((X_Max - X_Min) * 1.2f, 0.1f, (Z_Max - Z_Min) * 1.2f);

            /* Start Position */
            Ground.transform.position = new Vector3((X_Min + X_Max) / 2, Y_Min - 0.1f + this.Ground_Z_Offset, ((Z_Min + Z_Max) / 2));
        }

        private void Spawn_Link_Seg(Logic.Link_Segment Seg, Logic.Link Link)
        {
            float Seg_Len = (Seg.End - Seg.Start).magnitude;

            /* Spawn every lane separately */
            foreach (Logic.Lane Parent_Lane in Link.Lanes.Values.OrderByDescending(x => x.ID))
            {
                GameObject Lane = Logic.Spawner.Spawn("Road");

                Lane.transform.name = "Road_" + Link.ID + "_" + Seg.ID + "_" + Parent_Lane.ID;

                var Lane_Segment_Coords = Logic.Geometry.Utils.Get_Lane_Segment_Coordinates(Link, Seg, Parent_Lane, Seg_Len / 2);

                /* set startpoint (mid of target lane) */
                Lane.transform.position = new Vector3(Lane_Segment_Coords.Mid_Pos.x, Lane_Segment_Coords.Mid_Pos.y + (Lane_Height / 2), Lane_Segment_Coords.Mid_Pos.z);
                Lane.transform.rotation =
                    Quaternion.LookRotation(Lane_Segment_Coords.Vector_Along_Segment);

                // scale
                Lane.transform.localScale = new Vector3(Parent_Lane.Width, Lane_Height, Seg_Len);

                Update_Bound_Values(Lane_Segment_Coords.Min_Coord);
                Update_Bound_Values(Lane_Segment_Coords.Max_Coord);
                Seg.Lane_GameObj_Of_Link_Segment.Add(Lane);
            }

        }

        private void Spawn_Signals()
        {
            foreach (var Controller in this.Signal_Controllers)
            {
                // iterates through all signalgroups
                foreach (var Group in Controller.Value.Signal_Heads)
                {
                    // links.Key = link object, links.Value = signal object | ordered and grouped by the link
                    foreach (var Signal_Link in Group.Value.OrderBy(x => x.Value.Link.ID).GroupBy(x => x.Value.Link))
                    {
                        float Link_Segment_Len_To_Signal = 0.0f;

                        // in this loop it finds out on which linksegment the signal is and calculates the length from the beginning of link to the signal position
                        foreach (Logic.Link_Segment Seg in Signal_Link.Key.Link_Segments)
                        {
                            // length from start of the Link to the signal position 
                            Link_Segment_Len_To_Signal += Seg.XZ_Hyp;

                            Signal_Link.Key.Signals = new Dictionary<long, Signal.Head>();
                            float X_Kat = Seg.X_Kat;
                            float Y_Kat = Seg.Y_Kat;
                            float Z_Kat = Seg.Z_Kat;
                            float XZ_Hyp = Seg.XZ_Hyp;
                            float XYZ_Hyp = Seg.XYZ_Hyp;

                            // iterates through all signals which are on the target segment
                            foreach (Signal.Head Head in Signal_Link.Where(x => x.Value.Pos <= Link_Segment_Len_To_Signal && x.Value.Pos >= Link_Segment_Len_To_Signal - XYZ_Hyp).Select(x => x.Value).ToList())
                            {
                                float Signal_Height = 0.5f;

                                // lane where the target signal is on
                                Logic.Lane Parent_Lane = Signal_Link.Key.Lanes.Values.FirstOrDefault(x => x.ID == Head.Lane.ID);

                                // a link has collection with all signals on it
                                Signal_Link.Key.Signals.Add(Head.No, Head);

                                // unity game object for signals => Cube
                                Head.Obj = Logic.Spawner.Spawn("SignalHead");
                                Head.Obj.name = "SignalHead: " + Head.No + ", Link: " + Head.Link.ID + ", Lane: " + Head.Lane.ID;
                                Head.Colour = Color.red;
                                Head.UpdateState();

                                // GetSignalMapping returns => [0] worldcoordinate
                                //                             [1] signal start coordinate (left)
                                //                             [2] signal in mid of lane coordinate
                                //                             [3] signal end coordinate (right)

                                // offset from segment start to signal position => 
                                // formula: signal position - (length from start of link to End of target segment - length of target segment)
                                float Seg_Offset = Head.Pos - (Link_Segment_Len_To_Signal - XYZ_Hyp);

                                var Signal_Coord = Logic.Geometry.Utils.Get_Lane_Segment_Coordinates(Signal_Link.Key, Seg, Parent_Lane, Seg_Offset);

                                // set startpoint (mid of target lane)
                                Head.Obj.transform.position = new Vector3(Signal_Coord.Mid_Pos.x, Signal_Coord.Mid_Pos.y + (Signal_Height / 2), Signal_Coord.Mid_Pos.z);

                                Head.Obj.transform.rotation = Quaternion.LookRotation(Signal_Coord.Vector_Along_Segment);

                                // scale
                                Head.Obj.transform.localScale = new Vector3(Parent_Lane.Width, Signal_Height, 0.1f);

                                // BoxCollider is a unity component for physical settings
                                BoxCollider collider = Head.Obj.GetComponent<BoxCollider>();
                                // if enabled is true it blocks
                                collider.enabled = false;
                            }
                        }
                    }
                }
            }
        }
    }
}