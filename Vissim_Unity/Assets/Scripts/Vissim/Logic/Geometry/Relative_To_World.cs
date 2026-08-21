using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Vissim.Logic.Geometry
{
    public class Relative_To_World
    {
        
        /* This method calculates the world coordinates out of a relative Position on a given Link. */
        /* The world coordinates are given in the returned vector. */
        public static Vector3
        Calc_Vec_World_Coord(   Link    Link,
                                double  R_Pos)
        {
            float Sum_Of_Seg_Len = 0.0f;
            foreach (Link_Segment Cur_L_Seg in Link.Link_Segments)
            {
                float Len_Cur_L_Seg = (Cur_L_Seg.End - Cur_L_Seg.Start).magnitude;

                if (Sum_Of_Seg_Len + Len_Cur_L_Seg < R_Pos)
                    Sum_Of_Seg_Len += Len_Cur_L_Seg;
                else 
                {
                    float R_Pos_Position_On_Cur_L_Seg =
                        (float)R_Pos - Sum_Of_Seg_Len;

                    Vector3 Dir_Vec_Normalized =
                        (Cur_L_Seg.End - Cur_L_Seg.Start).normalized;

                    return (Dir_Vec_Normalized * R_Pos_Position_On_Cur_L_Seg + Cur_L_Seg.Start);
                }

            }
            
            /* should not happen, but if it happens it returns the Position of the end of the last Link_Segment. */
            return Link.Link_Segments[Link.Link_Segments.Count - 1].End;
        }
    };
}