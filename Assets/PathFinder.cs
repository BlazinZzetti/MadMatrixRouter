using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    public Point StartPoint;
    public Point EndPoint;

    public Button StartButton;
    public Button ResetButton;
    public Button StepButton;
    public Text CurrentPhaseText;
    public Dropdown SearchTypeDropdown;
    public Dropdown UpdateMethodDropdown;
    public Text CurrentDistanceToStartValueText;

    public float TimeToDelayInSeconds = 1.0f;
    private bool WaitingForDelayUpdateToFinish = false;   

    public enum UpdateMethod
    {
        Immeadiate,
        Delay,
        Manual
    }

    public UpdateMethod HowToUpdate = UpdateMethod.Manual;

    public DiijkstraSearch diijkstraSearch = new DiijkstraSearch();

    public AStarSearch aStarSeach = new AStarSearch();

    public void Start()
    {
        StartButton.onClick.RemoveAllListeners();
        ResetButton.onClick.RemoveAllListeners();
        StepButton.onClick.RemoveAllListeners();
        StartButton.onClick.AddListener(StartSearch);
        ResetButton.onClick.AddListener(ResetSearch);
        StepButton.onClick.AddListener(Step);
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
            if(HowToUpdate == UpdateMethod.Immeadiate)
            {
                Step();
            }
            else if(HowToUpdate == UpdateMethod.Delay)
            {
                StartCoroutine(DelayedStep());
            }
        }
    }

    private void StartSearch()
    {
        SearchTypeDropdown.interactable = false;
        var searchType = SearchTypeDropdown.value;
        switch (searchType)
        {
            case 0:
                diijkstraSearch.Setup(StartPoint, EndPoint);
                diijkstraSearch.Begin();
                break;
            case 1:
                aStarSeach.Setup(StartPoint, EndPoint);
                aStarSeach.Begin();
                break;
        }
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
        }

        foreach (var path in paths)
        {
            path.GetComponent<Path>().HighlightedNormalPath.SetActive(false);
        }

        SearchTypeDropdown.interactable = true;
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
}
