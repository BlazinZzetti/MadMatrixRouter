using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ACPoint : MonoBehaviour
{
    public GameObject Bomb;
    public GameObject PathPoint;
    public GameObject HighlightedPathPoint;
    public TextMesh ValueText;

    public float DistanceFromStart = -1;

    //Used in searching to backtrack.
    public ACPath shortestPathToThisPoint;

    public List<ACPath> Paths = new List<ACPath>();

    public float DistanceFromStartAStar(Point endPoint)
    {
        return DistanceFromStart + Vector3.Distance(transform.position, endPoint.transform.position);
    }

    public enum PointType
    {
        PathPoint,
        Bomb        
    }

    public PointType Type = PointType.PathPoint;

    private void Update()
    {
        ValueText.text = DistanceFromStart.ToString();
    }

    public static void CreateACPath(ACPoint pointA, ACPoint pointB, ACPath path)
    {
        path.PointA = pointA;
        path.PointB = pointB;

        pointA.Paths.Add(path);
        pointB.Paths.Add(path);
    }
}
