using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiijkstraSearch
{
    private Point startPoint;
    private Point endPoint;

    //The Point that is currently being used to look for other Points
    private Point currentPoint;

    //The Point that is currently being looked at to determine if it 
    //has already been used as "currentPoint" and to set DistanceFromStart
    private Point currentSearchPoint;

    /// <summary>
    /// List of Points that have already been processed.
    /// </summary>
    private List<Point> visitedPoints = new List<Point>();

    /// <summary>
    /// List of Points that have been touched, but not processed yet.
    /// </summary>
    private List<Point> unvisitedPoints = new List<Point>();

    /// <summary>
    /// Variable used to keep track of the 
    /// current index of a point's possible paths 
    /// </summary>
    private int pathIterator = 0;

    /// <summary>
    /// List of points that are the shortest path between the start and end points
    /// </summary>
    public List<Point> Result = new List<Point>();
    
    public enum SearchPhase
    {
        Wait,
        HighlightPoint,
        CheckPath,
        CheckPointOnPath,
        SwitchPathsOrPoint,
        Finish
    }

    public SearchPhase CurrentPhase = SearchPhase.Wait;

    /// <summary>
    /// Setup the start point the end point to be used for the search.
    /// </summary>
    /// <param name="start">Start Point</param>
    /// <param name="end">End Point</param>
    public void Setup(Point start, Point end)
    {
        startPoint = start;
        endPoint = end;        
    }

    /// <summary>
    /// Reset the search variable for the next search.
    /// </summary>
    public void Begin()
    {
        currentPoint = startPoint;
        currentSearchPoint = null;
        pathIterator = 0;
        CurrentPhase = SearchPhase.HighlightPoint;
        unvisitedPoints = new List<Point>();
        visitedPoints = new List<Point>();
        Result = new List<Point>();

        var Paths = GameObject.FindGameObjectsWithTag("Path");
        foreach (var path in Paths)
        {
            var currentPath = path.GetComponent<Path>();
            if (currentPath != null)
            {
                var pointA = currentPath.PointA;
                if (pointA != null && !pointA.Paths.Contains(currentPath))
                {
                    pointA.Paths.Add(currentPath);
                }

                var pointB = currentPath.PointB;
                if (pointB != null && !pointB.Paths.Contains(currentPath))
                {
                    pointB.Paths.Add(currentPath);
                }
            }
        }

        var Points = GameObject.FindGameObjectsWithTag("Point");
        foreach (var point in Points)
        {
            point.GetComponent<Point>().DistanceFromStart = -1;
        }
    }

    /// <summary>
    /// Perform a single step in the search.
    /// </summary>
    public void Step()
    {
        //if CurrentPhase == SearchPhase.Wait, do nothing.

        //Prepare currentPoint for checking paths connected to it.
        if (CurrentPhase == SearchPhase.HighlightPoint)
        {
            //Presentation: currentPoint is highlighted
            currentPoint.HighlightedPathPoint.SetActive(true);

            //Setup list of paths to go through
            //currentPoint.Paths.Sort(p => p.DistanceFromStart);

            //Find better way to sort in accending order.
            var pathsToSort = new List<Path>(currentPoint.Paths);

            for (int i = 0; i < pathsToSort.Count; i++)
            {
                for (int j = i + 1; j < pathsToSort.Count; j++)
                {
                    if (pathsToSort[i].OtherPoint(currentPoint).DistanceFromStart > pathsToSort[j].OtherPoint(currentPoint).DistanceFromStart)
                    {
                        var tempPath = pathsToSort[i];
                        pathsToSort[i] = pathsToSort[j];
                        pathsToSort[j] = tempPath;
                    }
                }
            }

            //Reassign currentPoint's paths with the ordered list version.
            currentPoint.Paths = new List<Path>(pathsToSort);

            //Reset pathInterator to start at first path in .Paths
            pathIterator = 0;

            //Move to CheckPath phase on next step.
            CurrentPhase = SearchPhase.CheckPath;
        }

        //Check a path connected to currentPoint based on the pathInterator to see if we are allowed to travel on it.
        else if (CurrentPhase == SearchPhase.CheckPath)
        {
            var pathToCheck = currentPoint.Paths[pathIterator];

            //If the path we are checking is a one way path, we need to make sure 
            //we are traveling across it in the direction it allows.
            if (pathToCheck.Type == Path.PathType.OneWay)
            {
                if(pathToCheck.OneWayDirection == Path.OneWayMode.AToB)
                {
                    if (pathToCheck.PointA == currentPoint)
                    {
                        //We are allow to continue processing this path
                        CurrentPhase = SearchPhase.CheckPointOnPath;
                    }
                    else
                    {
                        //We are not allow to process this path.
                        //Switch to the next path or if this is the last path, the next point.
                        CurrentPhase = SearchPhase.SwitchPathsOrPoint;
                    }
                }
                if (pathToCheck.OneWayDirection == Path.OneWayMode.BToA)
                {
                    if (pathToCheck.PointB == currentPoint)
                    {
                        //We are allow to continue processing this path
                        CurrentPhase = SearchPhase.CheckPointOnPath;
                    }
                    else
                    {
                        //We are not allow to process this path.
                        //Switch to the next path or if this is the last path, the next point.
                        CurrentPhase = SearchPhase.SwitchPathsOrPoint;
                    }
                }
            }
            else
            {
                //We are allow to continue processing this path
                CurrentPhase = SearchPhase.CheckPointOnPath;
            }
        }

        //Check if the point on the other side of the path has not been visited.
        //If not, calculate if this is the shortest path to the point so far.
        else if (CurrentPhase == SearchPhase.CheckPointOnPath)
        {
            //Use the path to aquire the Point on the other end of the path.
            currentSearchPoint = currentPoint.Paths[pathIterator].OtherPoint(currentPoint);

            //Presentation: Highlight the path we are currently looking at.
            currentPoint.Paths[pathIterator].HighlightedNormalPath.SetActive(true);

            //Check to see if this point has been proccessed as a currentPoint before continuing.
            if (!visitedPoints.Contains(currentSearchPoint))
            {
                //Presentation: Highlight currentSearchPoint
                currentSearchPoint.HighlightedPathPoint.SetActive(true);

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

                //If the point has not been assigned a value yet or if the point's current value is more than the current distance traveled.
                //Then: Assign the current distance traveled to DistanceFromStart.  Keep track of the path used to get to this point the fastest
                //so it can be used to backtrack and determine the Result List.
                if (currentSearchPoint.DistanceFromStart > pathDistance || currentSearchPoint.DistanceFromStart == -1)
                {
                    currentSearchPoint.DistanceFromStart = pathDistance;
                    currentSearchPoint.shortestPathToThisPoint = currentPoint.Paths[pathIterator];
                }
            }

            //Switch to the next path or if this is the last path, the next point.
            CurrentPhase = SearchPhase.SwitchPathsOrPoint;
        }

        //
        else if (CurrentPhase == SearchPhase.SwitchPathsOrPoint)
        {
            //Presentation: Unhighlight the current path and search point.
            //We may not have highlighted a search point, but we did highlight a path.
            if (currentSearchPoint != null)
            {
                currentSearchPoint.HighlightedPathPoint.SetActive(false);
            }
            currentPoint.Paths[pathIterator].HighlightedNormalPath.SetActive(false);

            //Incremet the pathIterator to check the next path on currentPoint
            pathIterator++;

            //If the pathIterator has a value that is >= the total number of paths connected to currentPoint,
            //then we have finished processing currentPoint and will need to pick the next currentPoint.
            if (pathIterator >= currentPoint.Paths.Count)
            {
                //Presentation: Unhighlight currentPoint
                currentPoint.HighlightedPathPoint.SetActive(false);

                //currentPoint is removed from the list of unvisted Points and is added to the list of visited Points.
                //This will prevent the search from doing processing the point again as we have done all we can with it.
                visitedPoints.Add(currentPoint);
                unvisitedPoints.Remove(currentPoint);

                //We'll add the Points that we looked at and assigned a DistanceFromStart value 
                //to the list of unvisted points which will be used to select the next currentPoint.
                foreach (var path in currentPoint.Paths)
                {
                    var canProcess = true;

                    if(path.Type == Path.PathType.OneWay)
                    {
                        if (path.OneWayDirection == Path.OneWayMode.AToB)
                        {
                            canProcess = path.PointA == currentPoint;
                        }
                        if (path.OneWayDirection == Path.OneWayMode.BToA)
                        {
                            canProcess = path.PointB == currentPoint;
                        }
                    }

                    if (canProcess)
                    {
                        var point = path.OtherPoint(currentPoint);

                        //If point has not been added to either list and is not the end Point.
                        if (!visitedPoints.Contains(point) && !unvisitedPoints.Contains(point) && point != endPoint)
                        {
                            unvisitedPoints.Add(point);
                        }
                    }
                }

                //Sort the list of unvistedPoints from lowest DistanceFromStart.
                //Points that have not been assigned a value are sent to the back of the list.
                for (int i = 0; i < unvisitedPoints.Count; i++)
                {
                    for (int j = i + 1; j < unvisitedPoints.Count; j++)
                    {
                        if (unvisitedPoints[i].DistanceFromStart > unvisitedPoints[j].DistanceFromStart
                        || unvisitedPoints[i].DistanceFromStart == -1 && unvisitedPoints[j].DistanceFromStart != -1)
                        {
                            var tempPoint = unvisitedPoints[i];
                            unvisitedPoints[i] = unvisitedPoints[j];
                            unvisitedPoints[j] = tempPoint;
                        }
                    }
                }

                //If there are no more unvisitedPoints, we dont need to search anymore.
                if(unvisitedPoints.Count > 0)
                {
                    //If endPoint.DistanceFromStart == -1, it means we havent hit it yet.
                    //If the first item in unvisitedPoints has a smaller DistanceFromStart, then there's a chance that there a faster way to the endpoint.
                    //If neither of these conditions are met, then we can assume that the shortest path has been found and we can stop searching.
                    if (unvisitedPoints[0].DistanceFromStart < endPoint.DistanceFromStart || endPoint.DistanceFromStart == -1)
                    {
                        currentPoint = unvisitedPoints[0];
                        CurrentPhase = SearchPhase.HighlightPoint;
                    }
                    else //if we hit the endpoint and no other points are shorter, we have reached the end.
                    {
                        CurrentPhase = SearchPhase.Finish;
                    }
                }                
                else
                {
                    CurrentPhase = SearchPhase.Finish;
                }
            }
            else
            {
                //Check the next path in currentPoint's list.
                CurrentPhase = SearchPhase.CheckPath;
            }
        }

        //Display the shortest path from the start point to the end point.
        else if (CurrentPhase == SearchPhase.Finish)
        {
            Result = new List<Point>();
            EndToStart(endPoint);
            //Highlight paths and points in results.
            for (int i = Result.Count - 1; i > 0; i--)
            {
                Result[i].HighlightedPathPoint.SetActive(true);
                if (Result[i].shortestPathToThisPoint != false)
                {
                    Result[i].shortestPathToThisPoint.HighlightedNormalPath.SetActive(true);
                }
            }
        }
    }
    /// <summary>
    /// Finds the shortest path from a point to the start point recurssively.
    /// It will populate Result with the shortest path from the inital point passed in to the startPoint.
    /// </summary>
    /// <param name="point">The point to travel backwards from.</param>
    void EndToStart(Point point)
    {
        if (point.shortestPathToThisPoint != null)
        {
            EndToStart(point.shortestPathToThisPoint.OtherPoint(point));
        }
        Result.Add(point);
    }
}
