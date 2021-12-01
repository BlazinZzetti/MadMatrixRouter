using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class ACPath : MonoBehaviour
{
    public int PathID;
    public GameObject NormalPath;
    public GameObject OneWayPath;
    public GameObject HighlightedNormalPath;
    public GameObject HighlightedOneWayPath;
    public TextMesh PathValueText;

    public ACPoint PointA;
    public ACPoint PointB;

    public float PathLength = -1f;
    public float PheromoneStrength = 1f;

    public enum PathType
    {
        Normal,
        OneWay
    }

    public enum OneWayMode
    {
        AToB,
        BToA
    }

    private PathType defaultPathType = PathType.Normal;
    public PathType Type = PathType.Normal;

    private OneWayMode defaultOneWayDirection = OneWayMode.AToB;
    public OneWayMode OneWayDirection = OneWayMode.AToB;

    private void Start()
    {
        defaultPathType = Type;
        defaultOneWayDirection = OneWayDirection;
    }

    private void Update()
    {
        calculatePath();
        if (Application.isPlaying && PathLength == -1)
        {
            //TempLength to get search working.
            PathLength = NormalPath.transform.localScale.x;
            if (PathValueText != null)
            {
                PathValueText.text = PathLength.ToString();
            }
        }
    }

    public void ResetToDefault()
    {
        Type = defaultPathType;
        OneWayDirection = defaultOneWayDirection;
    }

    void calculatePath()
    {
        if (PointA != null && PointB != null)
        {
            var startPoint = PointA.transform;
            var endPoint = PointB.transform;

            var midPoint = new Vector3(
                (startPoint.position.x + endPoint.position.x) / 2,
                (startPoint.position.y + endPoint.position.y) / 2, 
                (startPoint.position.z + endPoint.position.z) / 2);

            transform.position = midPoint;
            var distance = Vector3.Distance(startPoint.position, endPoint.position);

            SetLocalScaleForPaths(distance);

            Vector3 rotation = endPoint.position - transform.position;
            //Vector3 rotation = transform.position - endPoint.position;

            float angleY = Mathf.Atan2(rotation.z, rotation.x) * (180f / Mathf.PI);
            float angleZ = Mathf.Atan2(rotation.y, rotation.x) * (180f / Mathf.PI);

            if (angleY < 0)
                angleY += 360;

            if (angleZ < 0)
                angleZ += 360;

            transform.rotation = Quaternion.Euler(new Vector3(0, -angleY, angleZ));
        }
    }

    private void SetLocalScaleForPaths(float newLocalScale)
    {
        if (NormalPath != null)
        {
            NormalPath.transform.localScale = new Vector3(newLocalScale, 1, 1);
        }
        if (OneWayPath != null)
        {
            OneWayPath.transform.localScale = new Vector3(newLocalScale, 1, 1);
        }
        if (HighlightedNormalPath != null)
        {
            HighlightedNormalPath.transform.localScale = new Vector3(newLocalScale, 2.5f, 2.5f);
        }
        if (HighlightedOneWayPath != null)
        {
            HighlightedOneWayPath.transform.localScale = new Vector3(newLocalScale, 1.5f, 1.5f); ;
        }
    }

    public ACPoint OtherPoint(ACPoint currentPoint)
    {
        return (currentPoint != PointA) ? PointA : PointB;
    }
}
