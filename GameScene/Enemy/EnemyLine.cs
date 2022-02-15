using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float length;
    public float length1;
    public float length2;
    public float length3;
    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        length1 = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        length2 = Vector3.Distance(lineRenderer.GetPosition(1), lineRenderer.GetPosition(2));
        length3 = Vector3.Distance(lineRenderer.GetPosition(2), lineRenderer.GetPosition(3));
        length = length1 + length2 + length3;

    }

    public Vector3 GetPosition(float position)
    {
        var passed = length * position;
        if (passed <= length1) return Vector3.Lerp(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), passed / length1);
        if (length1 < passed && passed <= length1 + length2) return Vector3.Lerp(lineRenderer.GetPosition(1), lineRenderer.GetPosition(2), (passed - length1) / length2);
        if (length1 + length2 < passed) return Vector3.Lerp(lineRenderer.GetPosition(2), lineRenderer.GetPosition(3), (passed - length1 - length2) / length3);
        return new Vector3(0,0,0);
    }
}
