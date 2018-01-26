using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NetworkCorrectionToolWindow : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/NetworkCorrectionTool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        NetworkCorrectionToolWindow window = (NetworkCorrectionToolWindow)EditorWindow.GetWindow(typeof(NetworkCorrectionToolWindow));
        window.Show();
    }

    void OnGUI()
    {
        if(GUILayout.Button("Fix pathways"))
        {
            var Paths = GameObject.FindGameObjectsWithTag("Path");
            foreach (var path in Paths)
            {
                var currentPath = path.GetComponent<Path>();
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
        }
    }
}
