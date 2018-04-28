using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    public GameObject NormalPath;
    public GameObject OneWayPath;
    public GameObject HighlightedNormalPath;
    public GameObject HighlightedOneWayPath;
    public TextMesh PathValueText;

    public Point PointA;
    public Point PointB;

    public float PathLength = 1f;

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

    public PathType Type = PathType.Normal;

    public OneWayMode OneWayDirection = OneWayMode.AToB;

    private void Update()
    {
        calculatePath();
        if(PathValueText != null)
        {
            PathValueText.text = PathLength.ToString();
        }
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
            HighlightedNormalPath.transform.localScale = new Vector3(newLocalScale, 1.5f, 1.5f);
        }
        if (HighlightedOneWayPath != null)
        {
            HighlightedOneWayPath.transform.localScale = new Vector3(newLocalScale, 1.5f, 1.5f); ;
        }
    }

    public Point OtherPoint(Point currentPoint)
    {
        if (currentPoint != PointA)
        {
            return PointA;
        }
        return PointB;
    }
}
