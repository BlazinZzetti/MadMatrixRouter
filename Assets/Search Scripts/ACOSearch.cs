using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ACOSearch : MonoBehaviour
{
    public Button StartButton;
    public Button ResetButton;
    public Toggle ShortToggle;
    public Toggle PheromoneToggle;

    [Range(0f, 1f)]
    public float PheromoneEvaporation = 0;

    private ACPoint StartingPoint;
    private List<ACPoint> AllPoints;
    private List<ACPath> AllPaths;
    private List<Ant> ants = new List<Ant>();
    private bool search;
    private float shortestPathValue = float.MaxValue;
    private List<ACPoint> ShortestPath;

    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(StartSearch);
        ResetButton.onClick.AddListener(ResetPheromone);
        for (int i = 0; i < 10; i++)
        {
            ants.Add(new Ant());
        }
    }

    private void Update()
    {
        if(search)
        {
            //Have Ants Travel from point to point.
            for (int i = 0; i < ants.Count; i++)
            {
                ants[i].VisitedPoints.Add(StartingPoint);
                while (ants[i].VisitedPoints.Count < AllPoints.Count)
                {
                    ants[i].FindNextPoint();
                }
                ants[i].UpdateTotalDistanceTraveled();
            }

            //Pheromone Paths
            AllPaths.ForEach(p => p.PheromoneStrength = (1 - PheromoneEvaporation) * p.PheromoneStrength);

            foreach (var ant in ants)
            {
                if(ant.TotalDistanceTraveled < shortestPathValue)
                {
                    shortestPathValue = ant.TotalDistanceTraveled;
                    ShortestPath = new List<ACPoint>(ant.VisitedPoints);
                }
                ant.PheromonePaths();
                ant.Reset();
            }

            //Update rendering of the paths.
            var maxPheromone = 0f;
            foreach (var path in AllPaths)
            {
                if (path.PheromoneStrength > maxPheromone)
                {
                    maxPheromone = path.PheromoneStrength;
                }
            }
            foreach (var path in AllPaths)
            {
                path.HighlightedNormalPath.SetActive(PheromoneToggle.isOn);
                path.HighlightedNormalPath.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0, 1, 0.2965517f, path.PheromoneStrength / maxPheromone));
                path.NormalPath.SetActive(false);
            }

            for (int i = 1; i < ShortestPath.Count; i++)
            {
                var path = ShortestPath[i - 1].Paths.Find(p => p.OtherPoint(ShortestPath[i - 1]) == ShortestPath[i]
                                                            && p.OtherPoint(ShortestPath[i]) == ShortestPath[i - 1]);
                path.NormalPath.SetActive(ShortToggle.isOn);
            }
        }
    }

    private void ResetPheromone()
    {
        if (search)
        {
            search = false;
            AllPaths.ForEach(p =>
            {
                p.PheromoneStrength = 1;
                p.HighlightedNormalPath.SetActive(false);
                p.NormalPath.SetActive(true);
            });
            shortestPathValue = float.MaxValue;
            ShortestPath = new List<ACPoint>();
        }
    }

    private void StartSearch()
    {
        if (!search)
        {
            AllPoints = ((ACPoint[])FindObjectsOfType(typeof(ACPoint))).ToList();
            AllPaths = ((ACPath[])FindObjectsOfType(typeof(ACPath))).ToList();
            StartingPoint = AllPoints.Find(p => p.DistanceFromStart == 1);
            search = true;
        }
    }
}
