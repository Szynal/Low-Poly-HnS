using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PD_MakeSplineFromPoints))]
public class PD_MakeSplineFromPointsEditor : Editor {

    PD_MakeSplineFromPoints creator;
    PD_BezierSpline spline;

    public override void OnInspectorGUI() {

        creator = (PD_MakeSplineFromPoints) target;
        spline = creator.spline;

        DrawDefaultInspector();
        EditorGUILayout.Space();

        if(GUILayout.Button("Change waypoints to spline")) {

            changeWaypointsToSpline();
        }
    }

    void changeWaypointsToSpline() {

        for(int i = 2; i < creator.waypoints.Length; i++) {

            spline.InsertNewPointAt(i);
        }

        for(int i = 0; i < creator.waypoints.Length; i++) {

            spline.endPoints[i].position = creator.waypoints[i].position;
        }

        spline.AutoConstructSpline();
    }
}
