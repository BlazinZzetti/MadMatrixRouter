using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;

public class Ant
{
    public List<ACPoint> VisitedPoints = new List<ACPoint>();
    public float TotalDistanceTraveled;

    public void Reset()
    {
        VisitedPoints.Clear();
    }

    public void FindNextPoint()
    {
        Profiler.BeginSample("Ant - FindNextPoint");

        var currentPoint = VisitedPoints.Last();
        var pathLengths = new List<float>();
        var pathSum = 0f;
        var validPaths = new List<ACPath>();
        var choice = Random.Range(0f, 1f);

        //Gather data on each path to a valid next point.
        for(int i = 0; i < currentPoint.Paths.Count; i++)
        {
            //Slow :(
            if (!VisitedPoints.Contains(currentPoint.Paths[i].OtherPoint(currentPoint)))
            {
                validPaths.Add(currentPoint.Paths[i]);
                var pathValue = (currentPoint.Paths[i].PheromoneStrength) * (1 / currentPoint.Paths[i].PathLength);
                pathSum += pathValue;
                pathLengths.Add(pathValue);
            }
        }

        //Create a roultette wheel so a random 0-1 value can pick the next path.
        var rouletteWheel = new List<float>();
        for(int i = 0; i < pathLengths.Count; i++)
        {
            var value = pathLengths[i] / pathSum;
            if(i > 0)
            {
                value += rouletteWheel[i - 1];
            }
            rouletteWheel.Add(value);
        }

        //Get the next point based on the random choice.
        var pointIndex = -1;
        for(int i = 0; i < rouletteWheel.Count && pointIndex == -1; i++)
        {
            if (((i == 0) && (choice <= rouletteWheel[i]))
            || ((i > 0) && (choice > rouletteWheel[i - 1] && choice <= rouletteWheel[i])))
            {
                pointIndex = i;
            }
        }
        VisitedPoints.Add(validPaths[pointIndex].OtherPoint(currentPoint));

        Profiler.EndSample();
    }

    public void UpdateTotalDistanceTraveled()
    {
        //Update the total distance traveled.
        TotalDistanceTraveled = 0;
        for (int i = 1; i < VisitedPoints.Count; i++)
        {
            for(int j = 0; j < VisitedPoints[i - 1].Paths.Count; j++)
            {
                //Somehow also slow :(
                if(VisitedPoints[i - 1].Paths[j].OtherPoint(VisitedPoints[i - 1]) == VisitedPoints[i]
                && VisitedPoints[i - 1].Paths[j].OtherPoint(VisitedPoints[i]) == VisitedPoints[i - 1])
                {
                    TotalDistanceTraveled += VisitedPoints[i - 1].Paths[j].PathLength;
                    break;                    
                }
            }
        }
    }

    public void PheromonePaths()
    {
        for (int i = 1; i < VisitedPoints.Count; i++)
        {
            VisitedPoints[i - 1].Paths.Find(p => p.OtherPoint(VisitedPoints[i]) == VisitedPoints[i - 1]
                                           && p.OtherPoint(VisitedPoints[i - 1]) == VisitedPoints[i]).PheromoneStrength += (1 / TotalDistanceTraveled);
        }
    }
}