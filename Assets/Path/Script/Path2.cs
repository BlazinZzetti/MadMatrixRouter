using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Path2 : MonoBehaviour
{
    public GameObject NormalPath;
    public GameObject HighlightedPath;

    public Material normalPathColor;
    public Material OneWayPathColor;

    public Point PointA;
    public Point PointB;

    List<Point> TurnPoints = new List<Point>();

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
		if(Type == PathType.OneWay && OneWayDirection == OneWayMode.BToA)
		{
			CalculatePath(NormalPath.transform, HighlightedPath.transform, PointB.transform, PointA.transform);
		}
		else
		{
			CalculatePath(NormalPath.transform, HighlightedPath.transform, PointA.transform, PointB.transform);
        }
    }

    void CalculatePath(Transform path, Transform highlightedPath, Transform startPoint, Transform endPoint)
    {
        if (startPoint != null && endPoint != null)
        {
            var midPoint = new Vector3(
                (startPoint.position.x + endPoint.position.x) / 2,
                (startPoint.position.y + endPoint.position.y) / 2, 
                (startPoint.position.z + endPoint.position.z) / 2);

            transform.position = midPoint;
            path.position = midPoint;
            highlightedPath.position = midPoint;
            var distance = Vector3.Distance(startPoint.position, endPoint.position);

            SetLocalScaleForPaths(distance);

            Vector3 rotation = endPoint.position - startPoint.position;

            float angleY = Mathf.Atan2(rotation.z, rotation.x) * (180f / Mathf.PI);
            float angleZ = Mathf.Atan2(rotation.y, rotation.x) * (180f / Mathf.PI);

            if (angleY < 0)
                angleY += 360;

            if (angleZ < 0)
                angleZ += 360;

            path.rotation = Quaternion.Euler(new Vector3(0, -angleY, angleZ));
            highlightedPath.rotation = Quaternion.Euler(new Vector3(0, -angleY, angleZ));
        }
    }

    private void SetLocalScaleForPaths(float newLocalScale)
    {
        if (NormalPath != null)
        {
            NormalPath.transform.localScale = new Vector3(newLocalScale, 100, 100);
        }
        if (HighlightedPath != null)
        {
            HighlightedPath.transform.localScale = new Vector3(newLocalScale, 250f, 250f);
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