using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path2))]
public class Path2Editor : Editor
{
    private Path2 path;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (path != null)
        {
            //What type am I?
            //Use material for that type.
            if (path.Type == Path2.PathType.Normal)
            {
                path.NormalPath.GetComponent<MeshRenderer>().material = path.normalPathColor;
            }
            else
            {
                path.NormalPath.GetComponent<MeshRenderer>().material = path.OneWayPathColor;
            }
        }
        else
        {
            path = target as Path2;
        }

    }
}
