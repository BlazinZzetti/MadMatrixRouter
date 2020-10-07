using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    public Point StartPoint;
    public Point EndPoint;

    public Button StartButton;
    public Button ResetButton;
    public Button StepButton;
    public Button BombButton;
    public Text CurrentPhaseText;
    public Dropdown SearchTypeDropdown;
    public Dropdown UpdateMethodDropdown;
    public Text CurrentDistanceToStartValueText;
    public InputField Point1InputField;
    public InputField Point2InputField;

    public Button BombSetupButton;
    public Button ResetPathsButton;
    public InputField BombID1InputField;
    public InputField BombID2InputField; 
    public InputField BombSearchIndex1InputField;
    public InputField BombSearchIndex2InputField;

    public float TimeToDelayInSeconds = 1.0f;
    private bool WaitingForDelayUpdateToFinish = false;

    private List<MMPath> paths = new List<MMPath>();

    private BombData bombData = new BombData();

    public enum UpdateMethod
    {
        Immeadiate,
        Delay,
        Manual
    }

    public UpdateMethod HowToUpdate = UpdateMethod.Manual;

    public DiijkstraSearch diijkstraSearch = new DiijkstraSearch();

    public AStarSearch aStarSeach = new AStarSearch();

    private struct PathSettings
    {
        public MMPath.PathType pathType;
        public MMPath.OneWayMode oneWayMode;
    }

    private List<PathSettings> defaultPathSettings;

    public void Start()
    {
        StartButton.onClick.RemoveAllListeners();
        ResetButton.onClick.RemoveAllListeners();
        BombButton.onClick.RemoveAllListeners();
        StepButton.onClick.RemoveAllListeners();

        StartButton.onClick.AddListener(StartSearch);
        ResetButton.onClick.AddListener(ResetSearch);
        BombButton.onClick.AddListener(BombSearch);
        StepButton.onClick.AddListener(Step);
        BombSetupButton.onClick.AddListener(BombSetup);
        ResetPathsButton.onClick.AddListener(ResetPathsDefaults);

        paths = new List<MMPath>(GameObject.FindObjectsOfType<MMPath>());
        defaultPathSettings = new List<PathSettings>();

        foreach (var pathData in paths)
        {
            defaultPathSettings.Add(new PathSettings { pathType = pathData.Type, oneWayMode = pathData.OneWayDirection }); 
        }
    }

    private void Update()
    {
        var searchType = SearchTypeDropdown.value;
        switch (searchType)
        {
            case 0:
                CurrentPhaseText.text = "Current Phase: " + diijkstraSearch.CurrentPhase.ToString();
                CurrentDistanceToStartValueText.text = diijkstraSearch.CurrentBestDistanceToStart.ToString();
                break;
            case 1:
                CurrentPhaseText.text = "Current Phase: " + aStarSeach.CurrentPhase.ToString();
                CurrentDistanceToStartValueText.text = aStarSeach.CurrentBestDistanceToStart.ToString();
                break;
        }        

        //Check value of dropdown
        switch (UpdateMethodDropdown.value)
        {
            case 0:
                HowToUpdate = UpdateMethod.Manual;
                break;
            case 1:
                HowToUpdate = UpdateMethod.Delay;
                TimeToDelayInSeconds = 0.25f;
                break;
            case 2:
                HowToUpdate = UpdateMethod.Delay;
                TimeToDelayInSeconds = 0.5f;
                break;
            case 3:
                HowToUpdate = UpdateMethod.Delay;
                TimeToDelayInSeconds = 1f;
                break;
            case 4:
                HowToUpdate = UpdateMethod.Immeadiate;
                break;
        }

        if (HowToUpdate != UpdateMethod.Manual && !WaitingForDelayUpdateToFinish)
        {
            if (HowToUpdate == UpdateMethod.Immeadiate)
            {
                Step();
            }
            else if (HowToUpdate == UpdateMethod.Delay)
            {
                StartCoroutine(DelayedStep());
            }
        }
    }

    private void StartSearch()
    {
        StartSearchAsync(Point1InputField.text, Point2InputField.text);
    }

    private string StartSearch(string point1, string point2)
    {
        SearchTypeDropdown.interactable = false;
        Point1InputField.interactable = false;
        Point2InputField.interactable = false;

        StartPoint = GameObject.Find(point1).GetComponent<Point>();
        EndPoint = GameObject.Find(point2).GetComponent<Point>();

        foreach (var path in EndPoint.Paths)
        {
            if (path.Type != MMPath.PathType.OneWay)
            {
                path.Type = MMPath.PathType.OneWay;
                if (EndPoint == path.PointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
            }
        }

        diijkstraSearch.Setup(StartPoint, EndPoint);
        diijkstraSearch.Begin();

        return diijkstraSearch.CurrentBestDistanceToStart.ToString();
    }

    private string StartSearchAsync(string point1, string point2)
    {
        SearchTypeDropdown.interactable = false;
        Point1InputField.interactable = false;
        Point2InputField.interactable = false;

        StartPoint = GameObject.Find(point1).GetComponent<Point>();
        EndPoint = GameObject.Find(point2).GetComponent<Point>();

        //Set the default settings to start
        foreach (var path in paths)
        {
            path.ResetToDefault();
        }

        foreach (var path in EndPoint.Paths)
        {
            if (path.Type != MMPath.PathType.OneWay)
            {
                path.Type = MMPath.PathType.OneWay;
                if (EndPoint == path.PointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
            }
        }

        var searchType = SearchTypeDropdown.value;
        switch (searchType)
        {
            case 0:
                diijkstraSearch.Setup(StartPoint, EndPoint);
                diijkstraSearch.BeginAsync();
                break;
            case 1:
                aStarSeach.Setup(StartPoint, EndPoint);
                aStarSeach.BeginAsync();
                break;
        }

        return CurrentDistanceToStartValueText.text;
    }

    private void ResetSearch()
    {
        diijkstraSearch.CurrentPhase = DiijkstraSearch.SearchPhase.Wait;
        aStarSeach.CurrentPhase = DiijkstraSearch.SearchPhase.Wait;

        var points = GameObject.FindGameObjectsWithTag("Point");
        var paths = GameObject.FindGameObjectsWithTag("Path");

        foreach(var point in points)
        {
            point.GetComponent<Point>().DistanceFromStart = -1;
            point.GetComponent<Point>().HighlightedPathPoint.SetActive(false);
            point.GetComponent<Point>().shortestPathToThisPoint = null;
        }

        foreach (var path in paths)
        {
            path.GetComponent<MMPath>().HighlightedNormalPath.SetActive(false);
        }

        SearchTypeDropdown.interactable = true;
        Point1InputField.interactable = true;
        Point2InputField.interactable = true;
    }

    public void ResetPathsDefaults()
    {
        for (int i = 0; i < paths.Count; i++)
        {
            paths[i].Type = defaultPathSettings[i].pathType;
            paths[i].OneWayDirection = defaultPathSettings[i].oneWayMode;
        }
    }

    private IEnumerator DelayedStep()
    {
        if (HowToUpdate == UpdateMethod.Delay)
        {
            WaitingForDelayUpdateToFinish = true;
            Step();
            yield return new WaitForSeconds(TimeToDelayInSeconds);
        }
        WaitingForDelayUpdateToFinish = false;
    }

    private void Step()
    {
        var searchType = SearchTypeDropdown.value;
        switch (searchType)
        {
            case 0:
                diijkstraSearch.Step();
                break;
            case 1:
                aStarSeach.Step();
                break;
        }
    }

    private void BombSearch()
    {
        XmlDocument xmlDocument = new XmlDocument();
        HowToUpdate = UpdateMethod.Manual;

        // Set a variable to the Documents path.
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var mainNode = xmlDocument.CreateElement("Routes");

        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                //int i = 1; int j = 0;
                //While adding a check here for if we have done an i vs j comparision would make sense, 
                //we need to wait to filter that out later as one way paths may make going from i to j different than j to i.
                if (i != j) 
                {
                    var currentSearch = bombData.RetriveBombObjects(i, j);

                    if (currentSearch[0].StartPointB == 0)
                    {
                        if (currentSearch[1].StartPointB == 0 || currentSearch[1].isOneWay)
                        {
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 0, CompleteBombSearch(currentSearch, 0, 0));
                        }
                        else
                        {
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 0, CompleteBombSearch(currentSearch, 0, 0));
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 1, CompleteBombSearch(currentSearch, 0, 1));
                        }
                    }
                    else
                    {
                        if (currentSearch[1].StartPointB == 0 || currentSearch[1].isOneWay)
                        {
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 0, CompleteBombSearch(currentSearch, 0, 0));
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 1, 0, CompleteBombSearch(currentSearch, 1, 0));
                        }      
                        else   
                        {      
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 0, CompleteBombSearch(currentSearch, 0, 0));
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 0, 1, CompleteBombSearch(currentSearch, 0, 1));
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 1, 0, CompleteBombSearch(currentSearch, 1, 0));
                            CreateXMLRouteElement(xmlDocument, mainNode, i, j, 1, 1, CompleteBombSearch(currentSearch, 1, 1));
                        }
                    }
                }
            }
        }

        xmlDocument.AppendChild(mainNode);

        xmlDocument.Save(docPath + "\\MMReport.xml");
    }

    private void CreateXMLRouteElement(XmlDocument xmlDocument, XmlElement mainNode, int i, int j, int v1, int v2, string value)
    {
        var xmlElementRoute = xmlDocument.CreateElement("Route");
        xmlElementRoute.SetAttribute("StartBomb", i.ToString());
        xmlElementRoute.SetAttribute("EndBomb", j.ToString());
        xmlElementRoute.SetAttribute("StartIndex", v1.ToString());
        xmlElementRoute.SetAttribute("EndIndex", v2.ToString());
        xmlElementRoute.InnerText = value;
        mainNode.AppendChild(xmlElementRoute);
    }

    private string CompleteBombSearch(BombData.BombObject[] currentSearch, int v1, int v2)
    {
        //Starting point is pointA. Endpoint should be pointB.
        var pointA = (v1 == 0) ? currentSearch[0].StartPoint.ToString() : currentSearch[0].StartPointB.ToString();
        var pointB0 = currentSearch[1].StartPoint.ToString();
        var pointB1 = currentSearch[1].StartPointB.ToString();

        //Reset to default paths
        ResetSearch();
        ResetPathsDefaults();

        //Need to make adjustments to the map so that the only way to hit pointB, if on a path, is to go from 1 start point to the next.
        if (currentSearch[1].StartPointB != 0)
        {
            var pointB0Data = GameObject.Find(pointB0).GetComponent<Point>();
            var pointB1Data = GameObject.Find(pointB1).GetComponent<Point>();

            if (v2 == 0)
            {
                return test(pointA, pointB0Data, pointB1Data);
            }
            else
            {
                return test(pointA, pointB1Data, pointB0Data);
            }
        }
        else
        {
            //TODO: Still need to properly check if a path should be changed.
            //Dont want to change a one way path into something that cant happen.
            var endPoint = GameObject.Find(pointB0).GetComponent<Point>();
            foreach (var path in endPoint.Paths)
            {
                if (path.Type != MMPath.PathType.OneWay)
                {
                    var otherPoint = path.OtherPoint(endPoint);
                    var isOtherPointA = path.PointA == otherPoint;

                    path.Type = MMPath.PathType.OneWay;
                    if (isOtherPointA)
                    {
                        path.OneWayDirection = MMPath.OneWayMode.AToB;
                    }
                    else
                    {
                        path.OneWayDirection = MMPath.OneWayMode.BToA;
                    }
                }
            }
            return StartSearch(pointA, pointB0);
        }

        //Note: Technically we could have cases where 180 degree turns could happpen from one end to a new start,
        //but we'll filter that out manually as only a few should cause this before we find the fastest set of paths without it.
    }

    private void SetupBombSearch(BombData.BombObject[] currentSearch, int v1, int v2)
    {
        //Starting point is pointA. Endpoint should be pointB.
        var pointA = (v1 == 0) ? currentSearch[0].StartPoint.ToString() : currentSearch[0].StartPointB.ToString();
        var pointB0 = currentSearch[1].StartPoint.ToString();
        var pointB1 = currentSearch[1].StartPointB.ToString();

        //Reset to default paths
        ResetSearch();
        ResetPathsDefaults();

        //Need to make adjustments to the map so that the only way to hit pointB, if on a path, is to go from 1 start point to the next.
        if (currentSearch[1].StartPointB != 0)
        {
            var pointB0Data = GameObject.Find(pointB0).GetComponent<Point>();
            var pointB1Data = GameObject.Find(pointB1).GetComponent<Point>();

            if (v2 == 0)
            {
                test2(pointA, pointB0Data, pointB1Data);
            }
            else
            {
                test2(pointA, pointB1Data, pointB0Data);
            }
        }
        else
        {
            //TODO: Still need to properly check if a path should be changed.
            //Dont want to change a one way path into something that cant happen.
            var endPoint = GameObject.Find(pointB0).GetComponent<Point>();
            var pathsWithEndpoint = paths.FindAll(p => p.PointA == endPoint || p.PointB == endPoint);

            foreach (var path in pathsWithEndpoint)
            {
                if (path.Type != MMPath.PathType.OneWay)
                {
                    var otherPoint = path.OtherPoint(endPoint);
                    var isOtherPointA = path.PointA == otherPoint;

                    path.Type = MMPath.PathType.OneWay;
                    if (isOtherPointA)
                    {
                        path.OneWayDirection = MMPath.OneWayMode.AToB;
                    }
                    else
                    {
                        path.OneWayDirection = MMPath.OneWayMode.BToA;
                    }
                }
            }
        }

        //Note: Technically we could have cases where 180 degree turns could happpen from one end to a new start,
        //but we'll filter that out manually as only a few should cause this before we find the fastest set of paths without it.
    }

    public void BombSetup()
    {
        var currentSearch = bombData.RetriveBombObjects(int.Parse(BombID1InputField.text), int.Parse(BombID2InputField.text));
        SetupBombSearch(currentSearch, int.Parse(BombSearchIndex1InputField.text), int.Parse(BombSearchIndex2InputField.text));
    }

    private string test(string searchStartPoint, Point startPoint, Point endPoint)
    {
        //First force all normal paths to one way to point B1.
        var pathsWithEndpoint = paths.FindAll(p => p.PointA == endPoint || p.PointB == endPoint);

        foreach (var path in pathsWithEndpoint)
        {
            if (path.Type != MMPath.PathType.OneWay)
            {
                var otherPoint = path.OtherPoint(endPoint);
                var isOtherPointA = path.PointA == otherPoint;

                path.Type = MMPath.PathType.OneWay;
                if (isOtherPointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
            }
        }

        //Next force all normal paths away from B0
        //While searching make sure the path from B1 goes to B0.
        var pathsWithStartPoint = paths.FindAll(p => p.PointA == startPoint || p.PointB == startPoint);

        foreach (var path in pathsWithStartPoint)
        {
            if (path.OtherPoint(startPoint) == endPoint)
            {
                var isB0_PA = path.PointA == startPoint;
                if (path.Type == MMPath.PathType.OneWay)
                {
                    //Need to make sure that we both dont modify the path, and make sure we can legally move.
                    if (isB0_PA)
                    {
                        if (path.OneWayDirection != MMPath.OneWayMode.AToB)
                        {
                            //bad
                            return "Bad search";
                        }
                    }
                    else if (path.OneWayDirection != MMPath.OneWayMode.BToA)
                    {
                        //bad
                        return "Bad search";
                    }
                }
                else
                {
                    path.Type = MMPath.PathType.OneWay;

                    if (isB0_PA)
                    {
                        path.OneWayDirection = MMPath.OneWayMode.AToB;
                    }
                    else
                    {
                        path.OneWayDirection = MMPath.OneWayMode.BToA;
                    }
                }
            }
            else if (path.Type != MMPath.PathType.OneWay)
            {
                var otherPoint = path.OtherPoint(startPoint);
                var isOtherPointA = path.PointA == otherPoint;

                path.Type = MMPath.PathType.OneWay;
                if (isOtherPointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
            }
        }
        return StartSearch(searchStartPoint, endPoint.name);
    }

    private void test2(string searchStartPoint, Point startPoint, Point endPoint)
    {
        //First force all normal paths to one way to point B1.
        var pathsWithEndpoint = paths.FindAll(p => p.PointA == endPoint || p.PointB == endPoint);

        foreach (var path in pathsWithEndpoint)
        {
            if (path.Type != MMPath.PathType.OneWay)
            {
                var otherPoint = path.OtherPoint(endPoint);
                var isOtherPointA = path.PointA == otherPoint;

                path.Type = MMPath.PathType.OneWay;
                if (isOtherPointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
            }
        }

        //Next force all normal paths away from B0
        //While searching make sure the path from B1 goes to B0.
        var pathsWithStartPoint = paths.FindAll(p => p.PointA == startPoint || p.PointB == startPoint);

        foreach (var path in pathsWithStartPoint)
        {
            if (path.OtherPoint(startPoint) == endPoint)
            {
                var isB0_PA = path.PointA == startPoint;
                if (path.Type == MMPath.PathType.OneWay)
                {
                    //Need to make sure that we both dont modify the path, and make sure we can legally move.
                    if (isB0_PA)
                    {
                        if (path.OneWayDirection != MMPath.OneWayMode.AToB)
                        {
                            //bad
                            return;
                        }
                    }
                    else if (path.OneWayDirection != MMPath.OneWayMode.BToA)
                    {
                        //bad
                        return;
                    }
                }
                else
                {
                    path.Type = MMPath.PathType.OneWay;

                    if (isB0_PA)
                    {
                        path.OneWayDirection = MMPath.OneWayMode.AToB;
                    }
                    else
                    {
                        path.OneWayDirection = MMPath.OneWayMode.BToA;
                    }
                }
            }
            else if (path.Type != MMPath.PathType.OneWay)
            {
                var otherPoint = path.OtherPoint(startPoint);
                var isOtherPointA = path.PointA == otherPoint;

                path.Type = MMPath.PathType.OneWay;
                if (isOtherPointA)
                {
                    path.OneWayDirection = MMPath.OneWayMode.AToB;
                }
                else
                {
                    path.OneWayDirection = MMPath.OneWayMode.BToA;
                }
            }
        }
    }
}
