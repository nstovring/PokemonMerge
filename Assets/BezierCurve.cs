using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class BezierCurve : MonoBehaviour
{
    public List<Vector3> points;
    public float length;

    public void Initialize()
    {
        length = 0;
        int lineSteps = 20;
        Vector3 lineStart = GetPoint(0f);
        for (int i = 1; i <= lineSteps; i++)
        {
            Vector3 lineEnd = GetPoint(i / (float)lineSteps);
            length += Vector3.Distance(lineStart, lineEnd);
            lineStart = lineEnd;
        }
    }
    public void Show()
    {

    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(BezierCurve.GetPoint(points[0], points[1], points[2], t));
    }

    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(BezierCurve.GetFirstDerivative(points[0], points[1], points[2], t)) -
            transform.position;
    }
    //Analytical representation of bezier Polynomial interpolation
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    [ContextMenu("IntegerPoints")]
    public void IntegerPoints()
    {

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 newPoint = new Vector3((int)points[i].x, (int)points[i].y, (int)points[i].z);
            points[i] = newPoint;

        }
    }
}


