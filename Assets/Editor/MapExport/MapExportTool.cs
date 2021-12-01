using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Xml;
using System.Linq;

public class MapExportTool : EditorWindow
{
    private XmlDocument commandsXml;

    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/MapExportTool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MapExportTool window = (MapExportTool)EditorWindow.GetWindow(typeof(MapExportTool));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Assign Path IDs"))
        {
            assignPathIDs();
        }
        if (GUILayout.Button("Import File"))
        {
            importFile();
        }
        if (GUILayout.Button("Export File"))
        {
            exportFile();
        }
    }


    void assignPathIDs()
    {
        var Paths = GameObject.FindGameObjectsWithTag("Path");
        for (int i = 0; i < Paths.Length; i++)
        {
            Paths[i].GetComponent<MMPath>().PathID = i;
        }
    }

    void exportFile()
    {
        //Initialize Path connections to Points
        var Paths = GameObject.FindGameObjectsWithTag("Path");
        foreach (var path in Paths)
        {
            var currentPath = path.GetComponent<MMPath>();
            if (currentPath != null)
            {
                var pointA = currentPath.PointA;
                if (pointA != null && !pointA.Paths.Contains(currentPath))
                {
                    pointA.Paths.Add(currentPath);
                }

                var pointB = currentPath.PointB;
                if (pointB != null && !pointB.Paths.Contains(currentPath))
                {
                    pointB.Paths.Add(currentPath);
                }
            }
        }
        commandsXml = new XmlDocument();

        var rootElememt = commandsXml.CreateElement("MapData");

        var rootPointsElememt = commandsXml.CreateElement("Points");        

        var Points = GameObject.FindGameObjectsWithTag("Point");

        foreach(var point in Points)
        {
            var xmlElementPoint = commandsXml.CreateElement("Point");
            xmlElementPoint.SetAttribute("ID", point.name);
            var xmlElementPosition = commandsXml.CreateElement("Position");
            xmlElementPosition.SetAttribute("X", point.transform.position.x.ToString());
            xmlElementPosition.SetAttribute("Y", point.transform.position.y.ToString());
            xmlElementPosition.SetAttribute("Z", point.transform.position.z.ToString());
            var xmlElementConnectedPaths = commandsXml.CreateElement("ConnectedPaths");
            foreach (var path in point.GetComponent<Point>().Paths)
            {
                var xmlElementPath = commandsXml.CreateElement("PathID");
                xmlElementPath.InnerText = path.PathID.ToString();
                xmlElementConnectedPaths.AppendChild(xmlElementPath);
            }
            xmlElementPoint.AppendChild(xmlElementPosition);
            xmlElementPoint.AppendChild(xmlElementConnectedPaths);
            
            rootPointsElememt.AppendChild(xmlElementPoint);            
        }

        rootElememt.AppendChild(rootPointsElememt);        

        var rootPathElememt = commandsXml.CreateElement("Paths");

        foreach (var path in Paths)
        {
            var pathData = path.GetComponent<MMPath>();

            var xmlElementPath = commandsXml.CreateElement("Path");
            xmlElementPath.SetAttribute("ID", pathData.PathID.ToString());

            var xmlElementPointA = commandsXml.CreateElement("PointA");
            xmlElementPointA.InnerText = pathData.PointA.name;
            var xmlElementPointB = commandsXml.CreateElement("PointB");
            xmlElementPointB.InnerText = pathData.PointB.name;
            var xmlElementPathLength = commandsXml.CreateElement("PathLength");
            xmlElementPathLength.InnerText = pathData.PathLength.ToString();
            var xmlElementPathType = commandsXml.CreateElement("PathType");
            xmlElementPathType.InnerText = pathData.Type.ToString();
            var xmlElementPathOneWayDirection = commandsXml.CreateElement("OneWayDirection");
            xmlElementPathOneWayDirection.InnerText = pathData.OneWayDirection.ToString();

            xmlElementPath.AppendChild(xmlElementPointA);
            xmlElementPath.AppendChild(xmlElementPointB);
            xmlElementPath.AppendChild(xmlElementPathLength);
            xmlElementPath.AppendChild(xmlElementPathType);
            xmlElementPath.AppendChild(xmlElementPathOneWayDirection);

            rootPathElememt.AppendChild(xmlElementPath);
        }
        rootElememt.AppendChild(rootPathElememt);
        commandsXml.AppendChild(rootElememt);

        commandsXml.Save("XmlFiles/MapData.xml");
    }

    void importFile()
    {
        if (commandsXml == null)
        {
            commandsXml = new XmlDocument();
            commandsXml.Load("XmlFiles/MapData.xml");
        }

        foreach (XmlElement node in commandsXml.DocumentElement)
        {
            if (node.Name == "Points")
            {
                foreach (XmlNode pointNode in node.ChildNodes)
                {
                    var pointID = pointNode.Attributes[0].Value;

                    var pointPositionData = pointNode.ChildNodes[0];
                    var pointPosition = new Vector3(float.Parse(pointPositionData.Attributes[0].Value),
                                                    float.Parse(pointPositionData.Attributes[1].Value),
                                                    float.Parse(pointPositionData.Attributes[2].Value));
                    var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("Point")) as GameObject;

                    newPointObject.name = pointID;
                    newPointObject.transform.position = pointPosition;
                }
            }

            if (node.Name == "Paths")
            {
                var points = GameObject.FindGameObjectsWithTag("Point");
                var pointsList = new List<GameObject>(points);

                foreach (XmlNode pathNode in node.ChildNodes)
                {
                    var newPathObject = PrefabUtility.InstantiatePrefab(Resources.Load("Path")) as GameObject;
                    var newPath = newPathObject.GetComponent<MMPath>();

                    newPath.PointA = pointsList.First(p => p.name == pathNode.ChildNodes[0].InnerText).GetComponent<Point>();
                    newPath.PointB = pointsList.First(p => p.name == pathNode.ChildNodes[1].InnerText).GetComponent<Point>();
                    newPath.PathLength = float.Parse(pathNode.ChildNodes[2].InnerText);
                    newPath.Type = (MMPath.PathType)Enum.Parse(typeof(MMPath.PathType), pathNode.ChildNodes[3].InnerText);
                    newPath.OneWayDirection = (MMPath.OneWayMode)Enum.Parse(typeof(MMPath.OneWayMode), pathNode.ChildNodes[4].InnerText);
                }
            }
        }
    }

    ///// <summary>
    ///// Save data currently stored in commands to an XML file.
    ///// </summary>
    //public virtual void SaveCommands()
    //{
    //    commandsXml = new XmlDocument();
    //    commandsXml.AppendChild(commandsXml.CreateElement("Points"));
    //    foreach (var command in commands.Keys)
    //    {
    //        var xmlElementCommand = commandsXml.CreateElement("command");
    //        var xmlElementOutput = commandsXml.CreateElement("output");

    //        xmlElementCommand.SetAttribute("trigger", command);
    //        xmlElementOutput.InnerText = commands[command];

    //        xmlElementCommand.AppendChild(xmlElementOutput);
    //        commandsXml.DocumentElement.AppendChild(xmlElementCommand);
    //    }

    //    commandsXml.Save(CommandsXmlString);
    //}

    ///// <summary>
    ///// Load data from XML file to commands.
    ///// </summary>
    //protected virtual void LoadCommands()
    //{
    //    Dictionary<string, string> newCommands = new Dictionary<string, string>();

    //    if (commandsXml == null)
    //    {
    //        commandsXml = new XmlDocument();
    //        commandsXml.Load(CommandsXmlString);
    //    }

    //    foreach (XmlElement node in commandsXml.DocumentElement)
    //    {
    //        if (node.Name == "command")
    //        {
    //            var trigger = node.Attributes[0].Value;
    //            var output = node.ChildNodes[0].InnerText;
    //            newCommands.Add(trigger, output);
    //        }
    //    }
    //    commands = new Dictionary<string, string>(newCommands);
    //}

    //void onLoadFileLocationButtonPressed()
    //{
    //    System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

    //    var result = ofd.ShowDialog();

    //    if (result == System.Windows.Forms.DialogResult.OK && File.Exists(ofd.FileName))
    //    {
    //        // Open the file to read from.
    //        using (StreamReader sr = File.OpenText(ofd.FileName))
    //        {
    //            string s = "";
    //            for (int i = 0; i < 116; i++)
    //            {                    
    //                if ((s = sr.ReadLine()) != null)
    //                {
    //                    var splitS = s.Split(new char[] { '_' });
    //                    CreatePoint(int.Parse(splitS[0]), int.Parse(splitS[1]), int.Parse(splitS[2]));
    //                    if (int.Parse(splitS[1]) != 0)
    //                    {
    //                        CreatePoint(int.Parse(splitS[0]) + 116, -int.Parse(splitS[1]), int.Parse(splitS[2]));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void CreatePoint(int id, int xPos, int zPos)
    {
        var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("Point")) as GameObject;
        newPointObject.name = id.ToString();
        newPointObject.GetComponent<Point>().DistanceFromStart = id;
        newPointObject.transform.position = new Vector3(xPos, 0, zPos);
    }
}
