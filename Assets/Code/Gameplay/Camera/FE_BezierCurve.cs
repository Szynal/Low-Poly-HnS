using UnityEngine;

public static class Bezier
{
    public static Vector3 GetPoint(Vector3 _point0, Vector3 _point1, Vector3 _point2, float _t)
    {
        return Vector3.Lerp(Vector3.Lerp(_point0, _point1, _t), Vector3.Lerp(_point1, _point2, _t), _t);
    }
}

public class FE_BezierCurve : MonoBehaviour
{
    [SerializeField] private Vector3[] points;

    private Vector3[] startPoints;

    private void Awake()
    {
        startPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            startPoints[i] = points[i];
        }
    }

    public void Reset()
    {
        points = new[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }

    public Vector3 GetPoint(float _t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], _t));
    }

    public float GetLength()
    {
        Vector3 _midPoint = new Vector3((points[0].x + points[2].x) / 2f, (points[0].y + points[2].y) / 2f,
            (points[0].z + points[2].z) / 2f);
        return Vector3.Distance(points[0], points[2]) + Vector3.Distance(_midPoint, points[1]) / 2f;
    }

    public Vector3[] GetPoints()
    {
        return points;
    }

    public void SetPoint(int _index, Vector3 _target)
    {
        points[_index] = _target;
    }

    public void ResetPoints()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = startPoints[i];
        }
    }
}