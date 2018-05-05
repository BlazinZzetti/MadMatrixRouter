using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : DiijkstraSearch
{
    public override void CalculateDistanceFromStart()
    {
        //Initialize variables that will be used to determine the total path distance to currentSearchPoint.
        //Start with the distance it took to get from currentPoint to currentSearchPoint.
        var pathDistance = currentPoint.Paths[pathIterator].PathLength;

        //currentPoint will be the first point we travel backwards from.
        var pointForDistance = currentPoint;

        //Add up all paths up till this point.  Only the start point should not have a reference;
        while (pointForDistance.shortestPathToThisPoint != null)
        {
            pathDistance += pointForDistance.shortestPathToThisPoint.PathLength;
            pointForDistance = pointForDistance.shortestPathToThisPoint.OtherPoint(pointForDistance);
        }

        //A* override
        //Add the real world distance away from the end point to the pathDistace.
        pathDistance += Vector3.Distance(currentSearchPoint.transform.position, endPoint.transform.position);

        //If the point has not been assigned a value yet or if the point's current value is more than the current distance traveled.
        //Then: Assign the current distance traveled to DistanceFromStart.  Keep track of the path used to get to this point the fastest
        //so it can be used to backtrack and determine the Result List.
        if (currentSearchPoint.DistanceFromStart > pathDistance || currentSearchPoint.DistanceFromStart == -1)
        {
            currentSearchPoint.DistanceFromStart = pathDistance;
            currentSearchPoint.shortestPathToThisPoint = currentPoint.Paths[pathIterator];
        }
    }
}