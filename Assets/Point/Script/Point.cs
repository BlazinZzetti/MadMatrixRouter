using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Point : MonoBehaviour
{
    public GameObject Bomb;
    public GameObject PathPoint;
    public GameObject HighlightedPathPoint;
    public TextMesh ValueText;

    public float DistanceFromStart = -1;

    //Used in searching to backtrack.
    public MMPath shortestPathToThisPoint;

    public List<MMPath> Paths = new List<MMPath>();

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
}
