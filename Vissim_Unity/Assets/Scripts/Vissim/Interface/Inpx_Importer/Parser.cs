using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace Vissim.Interface
{
    public partial class Inpx_Importer
    {
        private const string LaneElement = "lane";
        private const string LinkPolyPointElement = "linkPolyPoint";
        private const string Point3DElement = "point3D";

        private Logic.Link Parse_Link(XmlNode linkNode, XmlDocument linkDocument)
        {
            var link = new Logic.Link
            {
                ID = ParseLong(linkDocument.DocumentElement, "no"),
                Level = ParseInt(linkNode, "level"),
                Visible = Display_Type_Visibilities[
                    ParseInt(linkNode, "displayType")
                ]
            };

            int laneId = 1;

            foreach (XmlNode laneNode in linkDocument.GetElementsByTagName(LaneElement))
            {
                var lane = new Logic.Lane
                {
                    ID = laneId,
                    Width = ParseFloat(laneNode, "width")
                };

                link.Lanes.Add(lane.ID, lane);
                laneId++;
            }

            Parse_Link_Segments(link, linkDocument);

            return link;
        }

        public Logic.Link Parse_Connector(XmlNode linkNode, XmlDocument linkDocument)
        {
            long connectorId = ParseLong(linkDocument.DocumentElement, "no");

            // Connector lanes are calculated after all links have been parsed.
            var connector = new Logic.Connector
            {
                ID = connectorId,
                Lane_Count = linkDocument.GetElementsByTagName(LaneElement).Count,
                Visible = Display_Type_Visibilities[
                    ParseInt(linkNode, "Display_Type")
                ]
            };

            XmlNode fromEndPoint = linkDocument
                .GetElementsByTagName("fromLinkEndPt")[0];

            XmlNode toEndPoint = linkDocument
                .GetElementsByTagName("toLinkEndPt")[0];

            // The lane attribute contains "<linkNo> <laneNo>".
            int[] fromLinkLane = ParseLinkLane(fromEndPoint);
            int[] toLinkLane = ParseLinkLane(toEndPoint);

            connector.From_Link_No = fromLinkLane[0];
            connector.From_Lane_No = fromLinkLane[1];

            connector.To_Link_No = toLinkLane[0];
            connector.To_Lane_No = toLinkLane[1];

            connector.From_Pos = ParseDouble(fromEndPoint, "pos");
            connector.To_Pos = ParseDouble(toEndPoint, "pos");

            Logic.Link link = connector;
            Parse_Link_Segments(link, linkDocument);

            return link;
        }

        public void Parse_Link_Segments(Logic.Link link, XmlDocument linkDocument)
        {
            float levelZOffset = GetLevelZOffset(link);
            XmlNodeList points = linkDocument.GetElementsByTagName(
                Get_Vissim_Version() > 10
                    ? LinkPolyPointElement
                    : Point3DElement);

            Vector3? previousPosition = null;
            int segmentId = 0;

            foreach (XmlNode pointNode in points)
            {
                Vector3 currentPosition = new Vector3(
                    ParseFloat(pointNode, "x"),
                    ParseFloat(pointNode, "zOffset") + levelZOffset,
                    ParseFloat(pointNode, "y"));

                if (previousPosition.HasValue)
                {
                    link.Link_Segments.Add(
                        new Logic.Link_Segment
                        {
                            ID = segmentId++,
                            Start = previousPosition.Value,
                            End = currentPosition
                        });
                }

                previousPosition = currentPosition;
            }
        }

        private float GetLevelZOffset(Logic.Link link)
        {
            if (link is Logic.Connector connector)
            {
                return Level_Z[Links[connector.From_Link_No].Level];
            }

            return Level_Z[link.Level];
        }

        private Dictionary<int, Route.Route> Parse_Routes(
            XmlDocument routingDecisionDocument,
            Route.Routing_Decision routingDecision,
            int routingDecisionId)
        {
            var routes = new Dictionary<int, Route.Route>();

            Logic.Link startLink = Links[routingDecision.Start_Link];
            bool startsOnConnector = startLink is Logic.Connector;

            foreach (XmlNode routeNode in routingDecisionDocument
                .GetElementsByTagName("vehicleRouteStatic"))
            {
                int routeId = ParseInt(routeNode, "no");
                int destinationLinkId = ParseInt(routeNode, "destLink");
                double destinationPosition = ParseDouble(routeNode, "destPos");

                Logic.Link destinationLink = Links[destinationLinkId];

                var route = new Route.Route
                {
                    Dest_Link = destinationLinkId,
                    Dest_Position =
                        Logic.Geometry.Relative_To_World.Calc_Vec_World_Coord(
                            destinationLink,
                            destinationPosition)
                };

                var routePoints = new List<Route.Point>();

                AddStartRoutePoint(
                    routePoints,
                    routingDecision);

                AddStartLinkRouteReferences(
                    startLink,
                    routingDecision.Pos,
                    routeId,
                    routingDecisionId);

                Vector3 previousConnectorEnd = Vector3.zero;
                bool firstRoutePoint = true;

                foreach (XmlNode routePointNode in routeNode.SelectNodes(".//intObjectRef"))
                {
                    int routePointLinkId = ParseInt(routePointNode, "key");
                    Logic.Link routePointLink = Links[routePointLinkId];

                    var routePoint = new Route.Point
                    {
                        Link = routePointLinkId
                    };

                    if (startsOnConnector && firstRoutePoint)
                    {
                        var startConnector = (Logic.Connector)startLink;

                        routePoint.Pos =
                            Logic.Geometry.Relative_To_World.Calc_Vec_World_Coord(
                                Links[startConnector.To_Link_No],
                                startConnector.To_Pos);
                    }
                    else if (routePointLink is Logic.Connector connector)
                    {
                        routePoint.Pos =
                            Logic.Geometry.Relative_To_World.Calc_Vec_World_Coord(
                                Links[connector.From_Link_No],
                                connector.From_Pos);

                        previousConnectorEnd =
                            Logic.Geometry.Relative_To_World.Calc_Vec_World_Coord(
                                Links[connector.To_Link_No],
                                connector.To_Pos);
                    }
                    else
                    {
                        routePoint.Pos = previousConnectorEnd;
                    }

                    AddRouteReferenceToSegments(
                        routePointLink,
                        routeId,
                        routingDecisionId);

                    routePoints.Add(routePoint);
                    firstRoutePoint = false;
                }

                AddDestinationRoutePoint(
                    route,
                    routePoints);

                AddDestinationLinkRouteReferences(
                    destinationLink,
                    routePoints[routePoints.Count - 2],
                    route.Dest_Position,
                    routeId,
                    routingDecisionId);

                route.Route_Points = routePoints;
                routes.Add(routeId, route);
            }

            return routes;
        }

        private void AddStartRoutePoint(
            List<Route.Point> routePoints,
            Route.Routing_Decision routingDecision)
        {
            routePoints.Add(
                new Route.Point
                {
                    Link = routingDecision.Start_Link,
                    Pos = routingDecision.Pos
                });
        }

        private void AddStartLinkRouteReferences(
            Logic.Link startLink,
            Vector3 startPosition,
            int routeId,
            int routingDecisionId)
        {
            Logic.Link_Segment startSegment =
                Get_Link_Segment_At_Position(
                    startLink.Link_Segments,
                    startPosition);

            var reference = new Route.Reference
            {
                Route = routeId,
                Decision = routingDecisionId,
                From_Offset_Pos = startPosition
            };

            bool reachedStartSegment = false;

            foreach (Logic.Link_Segment segment in startLink.Link_Segments)
            {
                if (segment == startSegment)
                {
                    reachedStartSegment = true;
                }

                if (reachedStartSegment)
                {
                    segment.Routes_Passing_Over.Add(reference);
                }
            }
        }

        private void AddRouteReferenceToSegments(
            Logic.Link link,
            int routeId,
            int routingDecisionId)
        {
            var reference = new Route.Reference
            {
                Route = routeId,
                Decision = routingDecisionId
            };

            foreach (Logic.Link_Segment segment in link.Link_Segments)
            {
                segment.Routes_Passing_Over.Add(reference);
            }
        }

        private void AddDestinationRoutePoint(
            Route.Route route,
            List<Route.Point> routePoints)
        {
            routePoints.Add(
                new Route.Point
                {
                    Link = route.Dest_Link,
                    Pos = route.Dest_Position
                });
        }

        private void AddDestinationLinkRouteReferences(
            Logic.Link destinationLink,
            Route.Point previousRoutePoint,
            Vector3 destinationPosition,
            int routeId,
            int routingDecisionId)
        {
            Logic.Link_Segment previousSegment =
                Get_Link_Segment_At_Position(
                    destinationLink.Link_Segments,
                    previousRoutePoint.Pos);

            Logic.Link_Segment destinationSegment =
                Get_Link_Segment_At_Position(
                    destinationLink.Link_Segments,
                    destinationPosition);

            var reference = new Route.Reference
            {
                Route = routeId,
                Decision = routingDecisionId,
                To_Offset_Pos = destinationPosition
            };

            bool routeHasStarted = false;

            foreach (Logic.Link_Segment segment in destinationLink.Link_Segments)
            {
                if (segment == previousSegment)
                {
                    routeHasStarted = true;
                }

                if (routeHasStarted)
                {
                    segment.Routes_Passing_Over.Add(reference);
                }

                if (segment == destinationSegment)
                {
                    break;
                }
            }
        }

        /*
         * Parses every <vehicleRoutingDecisionStatic> in the INPX file.
         */
        private void Parse_Vehicle_Routing_Decision_Statuc(
            XmlNode routingDecisionNode,
            XmlDocument routingDecisionDocument)
        {
            int routingDecisionId = ParseInt(routingDecisionNode, "no");
            int startLinkId = ParseInt(routingDecisionNode, "link");
            double relativePosition = ParseDouble(
                routingDecisionNode,
                "pos");

            var routingDecision = new Route.Routing_Decision
            {
                Start_Link = startLinkId,
                Pos =
                    Logic.Geometry.Relative_To_World.Calc_Vec_World_Coord(
                        Links[startLinkId],
                        relativePosition)
            };

            routingDecision.Routes = Parse_Routes(
                routingDecisionDocument,
                routingDecision,
                routingDecisionId);

            Routing_Decision_Dict.Add(
                routingDecisionId,
                routingDecision);
        }

        // ---------------------------------------------------------------------
        // XML helpers
        // ---------------------------------------------------------------------

        private static int ParseInt(XmlNode node, string attributeName)
        {
            return int.Parse(
                node.Attributes[attributeName].Value,
                CultureInfo.InvariantCulture);
        }

        private static long ParseLong(XmlNode node, string attributeName)
        {
            return long.Parse(
                node.Attributes[attributeName].Value,
                CultureInfo.InvariantCulture);
        }

        private static float ParseFloat(XmlNode node, string attributeName)
        {
            return float.Parse(
                node.Attributes[attributeName].Value,
                CultureInfo.InvariantCulture);
        }

        private static double ParseDouble(XmlNode node, string attributeName)
        {
            return double.Parse(
                node.Attributes[attributeName].Value,
                CultureInfo.InvariantCulture);
        }

        private static int[] ParseLinkLane(XmlNode node)
        {
            string value = node.Attributes["lane"].Value;

            return Array.ConvertAll(
                value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                int.Parse);
        }
    }
}