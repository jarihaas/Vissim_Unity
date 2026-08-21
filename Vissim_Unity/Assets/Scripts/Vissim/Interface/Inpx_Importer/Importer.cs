using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using Vissim.Logic;

namespace Vissim.Interface
{
    public partial class Inpx_Importer
    {
        public void Import()
        {
            Validate_Input_File();

            Initialize_Import_State();
            Load_XML();
            Parse_Simulation_Settings();

            Level_Z = Get_Level_Z();
            Display_Type_Visibilities = Read_Display_Type_Visibility();

            Parse_Links();
            Parse_Connectors();

            // Connector lane widths depend on all links having been parsed.
            Calc_Connector_Lane_Widths();

            Parse_Signal_Heads();
            Parse_Signal_Controllers();

            Spawn_Links();

            if (Fill_Seg_Gaps)
            {
                Fill_Segment_Spaces();
            }

            Spawn_Signals();
            Spawn_Ground();

            Parse_Routing_Decisions();
        }

        // =====================================================================
        // Import Setup
        // =====================================================================

        private void Validate_Input_File()
        {
            if (File.Exists(File_Name))
            {
                return;
            }

            throw new FileNotFoundException(
                "INPX file was not found.",
                File_Name);
        }

        private void Initialize_Import_State()
        {
            Links = new Dictionary<long, Logic.Link>();

            Starting_Pos = Vector3.zero;
            Starting_Orientation = Vector3.zero;
        }

        private void Load_XML()
        {
            XML = new XmlDocument();
            XML.Load(File_Name);
        }

        private void Parse_Simulation_Settings()
        {
            XmlNode Simulation =
                XML.GetElementsByTagName("simulation")[0];

            int Sim_Res =
                Parse_Int(Simulation, "simRes");

            Sim_Steps_In_Ticks =
                10000000L / Sim_Res;
        }

        // =====================================================================
        // Links
        // =====================================================================

        private void Parse_Links()
        {
            bool Return_Val = false;

            foreach (XmlNode Link in XML.GetElementsByTagName("link"))
            {
                XmlDocument C_Link = Create_Xml_Document(Link);

                // Connectors are parsed separately because they depend on
                // information from regular links.
                if (Current_Link_Is_Connector(C_Link))
                {
                    continue;
                }

                Logic.Link New_Link =
                    Parse_Link(Link, C_Link);

                if (!Return_Val)
                {
                    Set_Starting_Position(New_Link);
                    Return_Val = true;
                }

                Links.Add(New_Link.ID, New_Link);
            }
        }

        private void Parse_Connectors()
        {
            foreach (XmlNode Link in XML.GetElementsByTagName("link"))
            {
                XmlDocument C_Link = Create_Xml_Document(Link);

                if (!Current_Link_Is_Connector(C_Link))
                {
                    continue;
                }

                Logic.Link New_Link =
                    Parse_Connector(Link, C_Link);

                Links.Add(New_Link.ID, New_Link);
            }
        }

        private void Set_Starting_Position(Logic.Link New_Link)
        {
            Logic.Link_Segment First_Segment =
                New_Link.Link_Segments.FirstOrDefault();

            if (First_Segment == null)
            {
                return;
            }

            Starting_Pos =
                First_Segment.Start +
                (First_Segment.End - First_Segment.Start) * 0.5f;

            Starting_Orientation =
                (First_Segment.End - First_Segment.Start).normalized;
        }

        // =====================================================================
        // Signal Heads
        // =====================================================================

        private void Parse_Signal_Heads()
        {
            Signal_Heads =
                new Dictionary<long, Signal.Head>();

            foreach (XmlNode C_Signal_Head
                     in XML.GetElementsByTagName("Signal.Head"))
            {
                Parse_Signal_Head(C_Signal_Head);
            }
        }

        private void Parse_Signal_Head(XmlNode C_Signal_Head)
        {
            int[] Link_Lane =
                Parse_Pair(C_Signal_Head, "lane");

            int[] Group =
                Parse_Pair(C_Signal_Head, "sg");

            long Link_ID = Link_Lane[0];
            long Lane_ID = Link_Lane[1];

            Logic.Link Link =
                Get_Link(Link_ID);

            Logic.Lane Lane =
                Get_Lane(Link, Lane_ID);

            long Signal_ID =
                Parse_Long(C_Signal_Head, "no");

            Signal_Heads.Add(
                Signal_ID,
                new Signal.Head()
                {
                    Link = Link,
                    Lane = Lane,
                    No = Signal_ID,
                    Pos = Parse_Float(C_Signal_Head, "pos"),
                    Controller = Group[0],
                    SG = Group[1]
                });
        }

        // =====================================================================
        // Signal Controllers
        // =====================================================================

        private void Parse_Signal_Controllers()
        {
            Signal_Controllers =
                new Dictionary<long, Signal.Group>();

            foreach (XmlNode C_Signal_Controller
                     in XML.GetElementsByTagName("SignalController"))
            {
                Parse_Signal_Controller(C_Signal_Controller);
            }
        }

        private void Parse_Signal_Controller(
            XmlNode C_Signal_Controller)
        {
            long Controller_ID =
                Parse_Long(C_Signal_Controller, "no");

            var Signal_Groups =
                new Dictionary<long, Dictionary<long, Signal.Head>>();

            foreach (XmlNode C_Signal_Group
                     in C_Signal_Controller.SelectNodes("./Signal.Group"))
            {
                long Group_ID =
                    Parse_Long(C_Signal_Group, "no");

                Dictionary<long, Signal.Head> Signals =
                    Signal_Heads
                        .Where(x =>
                            x.Value.Controller == Controller_ID &&
                            x.Value.SG == Group_ID)
                        .ToDictionary(
                            x => x.Value.No,
                            x => x.Value);

                Signal_Groups.Add(
                    Group_ID,
                    Signals);
            }

            Signal.Group Group =
                new Signal.Group()
                {
                    Signal_Heads = Signal_Groups
                };

            Signal_Controllers.Add(
                Controller_ID,
                Group);
        }

        // =====================================================================
        // Routing Decisions
        // =====================================================================

        private void Parse_Routing_Decisions()
        {
            Routing_Decision_Dict =
                new Dictionary<int, Route.Routing_Decision>();

            foreach (XmlNode C_Routing_Decision
                     in XML.GetElementsByTagName(
                         "vehicleRoutingDecisionStatic"))
            {
                XmlDocument C_Routing_Decision_XML_Doc =
                    Create_Xml_Document(C_Routing_Decision);

                Parse_Vehicle_Routing_Decision_Statuc(
                    C_Routing_Decision,
                    C_Routing_Decision_XML_Doc);
            }
        }

        // =====================================================================
        // XML Helpers
        // =====================================================================

        private static XmlDocument Create_Xml_Document(XmlNode Node)
        {
            var Document = new XmlDocument();
            Document.LoadXml(Node.OuterXml);

            return Document;
        }

        private Logic.Link Get_Link(long Link_ID)
        {
            if (Links.TryGetValue(
                    Link_ID,
                    out Logic.Link Link))
            {
                return Link;
            }

            throw new KeyNotFoundException(
                $"Link {Link_ID} was referenced but could not be found.");
        }

        private static Logic.Lane Get_Lane(
            Logic.Link Link,
            long Lane_ID)
        {
            if (Link.Lanes.TryGetValue(
                    (int)Lane_ID,
                    out Logic.Lane Lane))
            {
                return Lane;
            }

            throw new KeyNotFoundException(
                $"Lane {Lane_ID} was not found on Link {Link.ID}.");
        }

        private static int[] Parse_Pair(
            XmlNode Node,
            string Attribute_Name)
        {
            string Value =
                Node.Attributes[Attribute_Name]?.Value;

            if (string.IsNullOrWhiteSpace(Value))
            {
                throw new InvalidDataException(
                    $"Missing '{Attribute_Name}' attribute.");
            }

            string[] Parts =
                Value.Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

            if (Parts.Length < 2)
            {
                throw new FormatException(
                    $"Expected two values in '{Attribute_Name}', " +
                    $"got '{Value}'.");
            }

            return new[]
            {
                int.Parse(
                    Parts[0],
                    CultureInfo.InvariantCulture),

                int.Parse(
                    Parts[1],
                    CultureInfo.InvariantCulture)
            };
        }

        private static int Parse_Int(
            XmlNode Node,
            string Attribute_Name)
        {
            return int.Parse(
                Node.Attributes[Attribute_Name].Value,
                CultureInfo.InvariantCulture);
        }

        private static long Parse_Long(
            XmlNode Node,
            string Attribute_Name)
        {
            return long.Parse(
                Node.Attributes[Attribute_Name].Value,
                CultureInfo.InvariantCulture);
        }

        private static float Parse_Float(
            XmlNode Node,
            string Attribute_Name)
        {
            return float.Parse(
                Node.Attributes[Attribute_Name].Value,
                CultureInfo.InvariantCulture);
        }
    }
}