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
    public Path shortestPathToThisPoint;

    public List<Path> Paths = new List<Path>();

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
