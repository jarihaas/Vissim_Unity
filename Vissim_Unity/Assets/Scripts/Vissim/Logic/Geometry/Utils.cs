using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Vissim.Logic.Geometry
{
    class Utils
    {
        public static Lane_Segment_Coordinates
        Get_Lane_Segment_Coordinates(   Link          Link,
                                        Link_Segment  Target_L_Seg,
                                        Lane          Parent_Lane,
                                        float               Offset_Pos_From_Seg_Start)
        {
            Lane_Segment_Coordinates Return_Value = new Lane_Segment_Coordinates();

            /* Vector Length from lane */
            Return_Value.Vector_Along_Segment = Target_L_Seg.End - Target_L_Seg.Start;

            /* Vector */
            Vector3 Vector_To_The_Left_Horizontal = new Vector3()
            {
                x = Return_Value.Vector_Along_Segment.normalized.z * (-1),
                y = 0,
                z = Return_Value.Vector_Along_Segment.normalized.x
            }.normalized;

            /* Calculates length to Start Point of the Lane Segment | Start is left side of the Parent Lane */
            float Len_To_Start_Pos_Target_Line = 0;
            foreach(var Lane in Link.Lanes.OrderByDescending(x => x.Value.ID))
            {
                if (Lane.Value.ID == Parent_Lane.ID)
                    break;
                Len_To_Start_Pos_Target_Line += Lane.Value.Width;
            }

            /* World Coordinate of the Lane Segment */
            Vector3 World_Coord =
                Target_L_Seg.Start + (Offset_Pos_From_Seg_Start * Return_Value.Vector_Along_Segment.normalized);

            /* Coordinate of the Left Border on the level of the World Coordinate */
            Vector3 Left_Side_Of_Link =
                World_Coord + (Vector_To_The_Left_Horizontal * Len_To_Start_Pos_Target_Line);
                
            /* Start Position */
            Vector3 Start_Pos =
                Left_Side_Of_Link + ((-1) * Vector_To_The_Left_Horizontal * Len_To_Start_Pos_Target_Line);

            /* Mid of Parent Lane */ 
            Return_Value.Mid_Pos = Start_Pos + ((-1) * Vector_To_The_Left_Horizontal * (Parent_Lane.Width / 2));

            /* min and max coordinates in 3d space */
            Return_Value.Min_Coord = Target_L_Seg.Start + Vector_To_The_Left_Horizontal * Parent_Lane.Width;
            Return_Value.Max_Coord = Target_L_Seg.End - Vector_To_The_Left_Horizontal * Parent_Lane.Width;

            return Return_Value;
        }
    }
}