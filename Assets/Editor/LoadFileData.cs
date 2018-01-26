using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

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
            onLoadFileLocationButtonPressed();
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

    void onLoadFileLocationButtonPressed()
    {
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

        var result = ofd.ShowDialog();

        if (result == System.Windows.Forms.DialogResult.OK && File.Exists(ofd.FileName))
        {
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(ofd.FileName))
            {
                string s = "";
                for (int i = 0; i < 116; i++)
                {                    
                    if ((s = sr.ReadLine()) != null)
                    {
                        var splitS = s.Split(new char[] { '_' });
                        CreatePoint(int.Parse(splitS[0]), int.Parse(splitS[1]), int.Parse(splitS[2]));
                        if (int.Parse(splitS[1]) != 0)
                        {
                            CreatePoint(int.Parse(splitS[0]) + 116, -int.Parse(splitS[1]), int.Parse(splitS[2]));
                        }
                    }
                }
            }
        }
    }

    public void CreatePoint(int id, int xPos, int zPos)
    {
        var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("Point")) as GameObject;
        newPointObject.name = id.ToString();
        newPointObject.GetComponent<Point>().DistanceFromStart = id;
        newPointObject.transform.position = new Vector3(xPos, 0, zPos);
    }
}
