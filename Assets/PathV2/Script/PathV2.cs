using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathV2 : MonoBehaviour
{
    public GameObject NormalPath;
    public GameObject HighlightedPath;

    public Material NormalPathColor;
    public Material OneWayPathColor;

    public PointV2 PointA;
    public PointV2 PointB;

    public float PathLength = 1f;

    public bool EditInEditMode = true;

    public GameObject SubPointsAndPaths;

    public List<PointV2> SubPoints = new List<PointV2>();
    public List<SubPath> SubPaths = new List<SubPath>();

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
#if UNITY_EDITOR

        if (EditInEditMode)
#endif
        {
            foreach (var path in SubPaths)
            {
                if (Type == PathType.OneWay)
                {
                    path.NormalPath.GetComponent<MeshRenderer>().material = OneWayPathColor;
                    if (OneWayDirection == OneWayMode.BToA)
                    {
                        path.CalculatePath(path.PointB.transform, path.PointA.transform);
                    }
                    else
                    {
                        path.CalculatePath(path.PointA.transform, path.PointB.transform);
                    }
                }
                else
                {
                    path.NormalPath.GetComponent<MeshRenderer>().material = NormalPathColor;
                    path.CalculatePath(path.PointA.transform, path.PointB.transform);
                }                
            }
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

            path.LookAt(endPoint);
            path.transform.localRotation *= Quaternion.Euler(0, 90, 0);
            
            //Vector3 rotation = endPoint.position - startPoint.position;

            //float angleY1 = Mathf.Atan2(-rotation.z, rotation.x);
            //float angleY = angleY1 * (180f / Mathf.PI);

            //if (angleY < 0)
            //{
            //    angleY += 360;
            //}

            //path.rotation = Quaternion.Euler(new Vector3(0, angleY, 0));
            //highlightedPath.rotation = Quaternion.Euler(new Vector3(0, angleY, 0));
        }
    }

    private void SetLocalScaleForPaths(float newLocalScale)
    {
        if (NormalPath != null)
        {
            NormalPath.transform.localScale = new Vector3(newLocalScale, .75f, .75f);
        }
        if (HighlightedPath != null)
        {
            HighlightedPath.transform.localScale = new Vector3(newLocalScale, 1f, 1f);
        }
    }

    public PointV2 OtherPoint(PointV2 currentPoint)
    {
        if (currentPoint != PointA)
        {
            return PointA;
        }
        return PointB;
    }
}