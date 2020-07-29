using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPath : MonoBehaviour
{
    public GameObject NormalPath;
    public GameObject HighlightedPath;

    public Material NormalPathColor;
    public Material OneWayPathColor;

    public PointV2 PointA;
    public PointV2 PointB;

    public float PathLength = 1f;

    public void CalculatePath(Transform startPoint, Transform endPoint)
    {
        if (startPoint != null && endPoint != null)
        {
            var midPoint = new Vector3(
                (startPoint.position.x + endPoint.position.x) / 2,
                (startPoint.position.y + endPoint.position.y) / 2,
                (startPoint.position.z + endPoint.position.z) / 2);

            transform.position = midPoint;
            NormalPath.transform.position = midPoint;
            HighlightedPath.transform.position = midPoint;

            var distance = Vector3.Distance(startPoint.position, endPoint.position);

            SetLocalScaleForPaths(distance);

            NormalPath.transform.LookAt(endPoint);
            NormalPath.transform.transform.localRotation *= Quaternion.Euler(0, 90, 0);

            HighlightedPath.transform.LookAt(endPoint);
            HighlightedPath.transform.transform.localRotation *= Quaternion.Euler(0, 90, 0);
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
}
