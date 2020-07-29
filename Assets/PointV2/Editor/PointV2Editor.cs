using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointV2))]
[CanEditMultipleObjects]
public class PointV2Editor : Editor
{
    private PointV2 point;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (point == null)
        {
            point = target as PointV2;
        }

        if (GUILayout.Button("Create New Point"))
        {
            createPoint();
        }

        if (Selection.gameObjects.Length == 2)
        {
            if (GUILayout.Button("Connect Selected Points with Path"))
            {
                createPath();
            }
        }
    }

    private void createPoint()
    {
        if (point != null)
        {
            var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("PointV2")) as GameObject;
            var newPathObject = PrefabUtility.InstantiatePrefab(Resources.Load("PathV2")) as GameObject;

            var newPath = newPathObject.GetComponent<PathV2>();
            var newPoint = newPointObject.GetComponent<PointV2>();
            newPath.PointA = point;
            newPath.PointB = newPoint;
            newPoint.transform.position = point.transform.position;
            point.Paths.Add(newPath);
            newPoint.Paths.Add(newPath);
            Selection.activeGameObject = newPointObject;
        }
    }

    private void createPath()
    {
        if (Selection.gameObjects.Length == 2)
        {
            var pointAObject = Selection.gameObjects[0];
            var pointBObject = Selection.gameObjects[1];

            var pointA = pointAObject.GetComponent<PointV2>();
            var pointB = pointBObject.GetComponent<PointV2>();

            var newPathObject = PrefabUtility.InstantiatePrefab(Resources.Load("PathV2")) as GameObject;

            var newPath = newPathObject.GetComponent<PathV2>();

            newPath.PointA = pointA;
            newPath.PointB = pointB;

            pointA.Paths.Add(newPath);
            pointB.Paths.Add(newPath);
        }
    }
}