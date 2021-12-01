using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Linq;

public class LoadFileData : EditorWindow
{
    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/LoadFileData")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LoadFileData window = (LoadFileData)EditorWindow.GetWindow(typeof(LoadFileData));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Load File"))
        {
            //onLoadFileLocationButtonPressed();
        }

        if (GUILayout.Button("Load Bonb File"))
        {
            //onLoadBombFileLocationButtonPressed();
        }

        if (GUILayout.Button("Fix Distance To Start"))
        {
            var Points = GameObject.FindGameObjectsWithTag("Point");
            foreach (var point in Points)
            {
                var currentpoint = point.GetComponent<Point>();
                if (currentpoint.DistanceFromStart > 116)
                {
                    currentpoint.DistanceFromStart -= 116;
                }
            }
        }
    }

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

    //void onLoadBombFileLocationButtonPressed()
    //{
    //    System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

    //    var result = ofd.ShowDialog();

    //    if (result == System.Windows.Forms.DialogResult.OK && File.Exists(ofd.FileName))
    //    {
    //        var allPoints = new List<ACPoint>();
    //        var pointsParent = GameObject.Find("ACPoints");
    //        if(pointsParent == null)
    //        {
    //            pointsParent = Instantiate(new GameObject());
    //            pointsParent.name = "ACPoints";
    //        }
    //        // Open the file to read from.
    //        using (StreamReader sr = File.OpenText(ofd.FileName))
    //        {
    //            string s = "";
    //            for (int i = 1; i <= 30; i++)
    //            {
    //                if ((s = sr.ReadLine()) != null)
    //                {
    //                    var splitS = s.Split(new char[] { ',' });
    //                    var acPoint = CreateACPoint(i, int.Parse(splitS[0]) / 50, int.Parse(splitS[1]) / 50);
    //                    acPoint.transform.parent = pointsParent.transform;
    //                    allPoints.Add(acPoint);
    //                }
    //            }
    //        }
    //        for(int i = 0; i < allPoints.Count; i++)
    //        {
    //            var currentPoint = allPoints[i];

    //            var pathParent = GameObject.Find("ACPaths");
    //            if (pathParent == null)
    //            {
    //                pathParent = Instantiate(new GameObject());
    //                pathParent.name = "ACPaths";
    //            }

    //            foreach (var acp in allPoints)
    //            {
    //                if(acp.DistanceFromStart != currentPoint.DistanceFromStart && !currentPoint.Paths.Any(p => p.OtherPoint(currentPoint) == acp))
    //                {
    //                    var path = PrefabUtility.InstantiatePrefab(Resources.Load("ACPath")) as GameObject;
    //                    path.transform.parent = pathParent.transform;
    //                    ACPoint.CreateACPath(currentPoint, acp, path.GetComponent<ACPath>());
    //                }
    //            }
    //        }
    //    }
    //}

    public void CreatePoint(int id, int xPos, int zPos)
    {
        var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("Point")) as GameObject;
        newPointObject.name = id.ToString();
        newPointObject.GetComponent<ACPoint>().DistanceFromStart = id;
        newPointObject.transform.position = new Vector3(xPos, 0, zPos);
    }

    public ACPoint CreateACPoint(int id, int xPos, int zPos)
    {
        var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("ACPoint")) as GameObject;
        newPointObject.name = id.ToString();
        newPointObject.GetComponent<ACPoint>().DistanceFromStart = id;
        newPointObject.transform.position = new Vector3(xPos, 0, zPos);
        return newPointObject.GetComponent<ACPoint>();
    }
}
