using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ACPath))]
public class ACPathEditor : Editor
{
    private ACPath path;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (path != null)
        {
        //    path.NormalPath = EditorGUILayout.ObjectField("Normal Path", path.NormalPath, typeof(GameObject), true) as GameObject;
        //    path.OneWayPath = EditorGUILayout.ObjectField("One Way Path", path.OneWayPath, typeof(GameObject), true) as GameObject;
        //    path.HighlightedNormalPath = EditorGUILayout.ObjectField("Highlighted Normal Path", path.HighlightedNormalPath, typeof(GameObject), true) as GameObject;
        //    path.pathLength = EditorGUILayout.IntField("Path Length", path.pathLength);

        //    path.Type = (Path.PathType)EditorGUILayout.EnumPopup("Path Type", path.Type);

        //    if (path.Type == Path.PathType.OneWay)
        //    {
        //        path.OneWayDirection = (Path.OneWayMode)EditorGUILayout.EnumPopup("One Way Direction", path.OneWayDirection);
        //    }

            //What type am I?
            //Switch Object to that type.
            if (path.Type == ACPath.PathType.Normal)
            {
                path.NormalPath.SetActive(true);
                path.OneWayPath.SetActive(false);
            }
            else
            {
                path.NormalPath.SetActive(false);
                path.OneWayPath.SetActive(true);
            }
        }
        else
        {
            path = target as ACPath;
        }

    }
}
