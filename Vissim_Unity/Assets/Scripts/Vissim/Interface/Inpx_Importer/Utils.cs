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
        /* Updates_Minimum and Maximum values */
        private void Update_Bound_Values(Vector3 C_Val)
        {
            X_Min = X_Min < C_Val.x ? X_Min : C_Val.x;
            X_Max = X_Max > C_Val.x ? X_Max : C_Val.x;
            Y_Min = Y_Min < C_Val.z ? Y_Min : C_Val.z;
            Z_Max = Z_Max > C_Val.z ? Z_Max : C_Val.z;
            Z_Min = Z_Min < C_Val.y ? Z_Min : C_Val.y;
        }
        private void Fill_Segment_Spaces()
        {
            foreach (var Link in this.Links)
            {
                foreach (var Seg in Link.Value.Link_Segments)
                {
                    GameObject Seg_Filler = Logic.Spawner.Spawn("SegmentFiller");
                    Seg_Filler.name = "SegmentFiller " + Link.Value.ID;
                    Seg_Filler.transform.position = Seg.Start;
                    Seg_Filler.transform.localScale = new Vector3(Link.Value.Get_Lane_Widths().Sum(), Lane_Height * 0.99f, Link.Value.Get_Lane_Widths().Sum());
                }
            }
        }

        private void Calc_Connector_Lane_Widths()
        {
            foreach (Logic.Link Connector in Links.Values.Where(l => l.GetType() == typeof(Logic.Connector)))
            {
                Logic.Connector C = (Logic.Connector)Connector;
                float From_W = Links[C.From_Link_No].Lanes[C.From_Lane_No].Width;
                float To_W = Links[C.To_Link_No].Lanes[C.To_Lane_No].Width;

                for (int i = 1; i <= C.Lane_Count; ++i)
                {
                    Logic.Lane New_Lane = new Logic.Lane();
                    New_Lane.Width = (From_W + To_W) / 2;
                    New_Lane.ID = i;
                    Connector.Lanes.Add(i, New_Lane);
                }
            }
        }

        private Logic.Link_Segment Get_Link_Segment_At_Position(List<Logic.Link_Segment> Segments, Vector3 Pos)
        {
            foreach (var C_Seg in Segments)
            {
                if ((Pos - C_Seg.Start).magnitude <= (C_Seg.End - C_Seg.Start).magnitude)
                {
                    return C_Seg;
                }
            }
            return null;
        }

        private int Get_Vissim_Version()
        {
            XmlNode Network = this.XML.GetElementsByTagName("network")[0];
            string Version_Unformatted = Network.Attributes["vissimVersion"].Value.Split('.')[0];
            int vissimVersion = int.Parse(Version_Unformatted);
            return vissimVersion;
        }

        private Dictionary<int, float> Get_Level_Z()
        {
            Dictionary<int, float> Z = new Dictionary<int, float>();
            foreach (XmlNode Level in this.XML.GetElementsByTagName("level"))
            {
                int No = int.Parse(Level.Attributes["no"].Value);
                float Z_Coord = float.Parse(Level.Attributes["zCoord"].Value);
                Z[No] = Z_Coord;
            }
            return Z;
        }

        private Dictionary<int, bool> Read_Display_Type_Visibility()
        {
            Dictionary<int, bool> Display_Type_Visibility = new Dictionary<int, bool>();
            foreach (XmlNode displayType in this.XML.GetElementsByTagName("displayType"))
            {
                int No = int.Parse(displayType.Attributes["No"].Value);
                bool Visible = !bool.Parse(displayType.Attributes["invisible"].Value);
                Display_Type_Visibility[No] = Visible;
            }
            return Display_Type_Visibility;
        }

        private bool Current_Link_Is_Connector(XmlDocument Current_Link)
        {
            return (Current_Link.GetElementsByTagName("fromLinkEndPt").Count > 0 || Current_Link.GetElementsByTagName("toLinkEndPt").Count > 0);
        }
    }
}