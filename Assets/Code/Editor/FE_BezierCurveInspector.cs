using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FE_BezierCurve))]
public class FE_BezierCurveInspector : Editor
{
    private FE_BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private int lineSteps = 10;

    private Transform pos1;
    private Transform pos2;

    public override void OnInspectorGUI()
    {
        pos1 =
            EditorGUILayout.ObjectField("Start position", pos1, typeof(Transform),
                allowSceneObjects: true) as Transform;
        pos2 =
            EditorGUILayout.ObjectField("End position", pos2, typeof(Transform), allowSceneObjects: true) as Transform;

        if (pos1 != null || pos2 != null)
        {
            if (GUILayout.Button("Use indicated transforms"))
            {
                curve = target as FE_BezierCurve;

                if (pos1 != null)
                {
                    curve.SetPoint(0, pos1.position);
                }

                if (pos2 != null)
                {
                    curve.SetPoint(2, pos2.position);
                }

                curve.SetPoint(1, Vector3.Lerp(curve.GetPoint(0), curve.GetPoint(2), 0.5f));

                EditorUtility.SetDirty(curve.gameObject);
            }
        }
    }

    private void OnSceneGUI()
    {
        curve = target as FE_BezierCurve;
        handleTransform = curve.transform;
        if (Tools.pivotRotation == PivotRotation.Local)
            handleRotation = handleTransform.rotation;
        else
            handleRotation = Quaternion.identity;

        Vector3 _point0 = showPoint(0);
        Vector3 _point1 = showPoint(1);
        Vector3 _point2 = showPoint(2);

        Handles.color = Color.gray;
        Handles.DrawLine(_point0, _point1);
        Handles.DrawLine(_point1, _point2);

        Handles.color = Color.white;
        Vector3 lineStart = curve.GetPoint(0f);
        for (int i = 1; i <= lineSteps; i++)
        {
            Vector3 lineEnd = curve.GetPoint(i / (float) lineSteps);
            Handles.DrawLine(lineStart, lineEnd);
            lineStart = lineEnd;
        }
    }

    private Vector3 showPoint(int _index)
    {
        Vector3 _ret = handleTransform.TransformPoint(curve.GetPoints()[_index]);
        EditorGUI.BeginChangeCheck();
        _ret = Handles.DoPositionHandle(_ret, handleRotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move point");
            EditorUtility.SetDirty(curve);
            curve.GetPoints()[_index] = handleTransform.InverseTransformPoint(_ret);
        }

        return _ret;
    }
}