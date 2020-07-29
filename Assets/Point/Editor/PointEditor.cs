using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Point))]
[CanEditMultipleObjects]
public class PointEditor : Editor
{
    private Point point;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

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

        if (point != null)
        {
            //What type am I?
            //Switch Object to that type.
            if (point.Type == Point.PointType.PathPoint)
            {
                point.PathPoint.SetActive(true);
                point.Bomb.SetActive(false);
            }
            else
            {
                point.PathPoint.SetActive(false);
                point.Bomb.SetActive(true);
            }
        }
        else
        {
            point = target as Point;
        }
    }

    private void createPoint()
    {
        if (point != null)
        {
            var newPointObject = PrefabUtility.InstantiatePrefab(Resources.Load("Point")) as GameObject;
            var newPathObject = PrefabUtility.InstantiatePrefab(Resources.Load("Path")) as GameObject;

            var newPath = newPathObject.GetComponent<MMPath>();
            var newPoint = newPointObject.GetComponent<Point>();
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

            var pointA = pointAObject.GetComponent<Point>();
            var pointB = pointBObject.GetComponent<Point>();

            var newPathObject = PrefabUtility.InstantiatePrefab(Resources.Load("Path")) as GameObject;

            var newPath = newPathObject.GetComponent<MMPath>();

            newPath.PointA = pointA;
            newPath.PointB = pointB;

            pointA.Paths.Add(newPath);
            pointB.Paths.Add(newPath);
        }
    }
}