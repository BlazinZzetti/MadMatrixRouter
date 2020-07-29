using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PointV2 : MonoBehaviour
{
    public GameObject PathPoint;
    public GameObject HighlightedPathPoint;
    public TextMesh ValueText;

    public float DistanceFromStart = -1;

    //Used in searching to backtrack.
    public PathV2 shortestPathToThisPoint;

    public List<PathV2> Paths = new List<PathV2>();

    public float DistanceFromStartAStar(PointV2 endPoint)
    {
        return DistanceFromStart + Vector3.Distance(transform.position, endPoint.transform.position);
    }

    private void Update()
    {
        //ValueText.text = DistanceFromStart.ToString();
    }
}
