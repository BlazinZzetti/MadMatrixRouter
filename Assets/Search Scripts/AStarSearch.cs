using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : DiijkstraSearch
{
    public override void SortUnvisited()
    {
        for (int i = 0; i < unvisitedPoints.Count; i++)
        {
            for (int j = i + 1; j < unvisitedPoints.Count; j++)
            {
                var trueDistanceI = unvisitedPoints[i].DistanceFromStart + Vector3.Distance(unvisitedPoints[i].transform.position, endPoint.transform.position);
                var trueDistancej = unvisitedPoints[j].DistanceFromStart + Vector3.Distance(unvisitedPoints[j].transform.position, endPoint.transform.position);

                if (trueDistanceI > trueDistancej || unvisitedPoints[i].DistanceFromStart == -1 && unvisitedPoints[j].DistanceFromStart != -1)
                {
                    var tempPoint = unvisitedPoints[i];
                    unvisitedPoints[i] = unvisitedPoints[j];
                    unvisitedPoints[j] = tempPoint;
                }
            }
        }
    }
}