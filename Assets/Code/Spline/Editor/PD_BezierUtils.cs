using UnityEngine;
using UnityEditor;

public static class PD_BezierUtils {

	private const int SPLINE_GIZMO_SMOOTHNESS = 10;

	private static Color NORMAL_END_POINT_COLOR = new Color(0.22f, 0.88f, 0.9f);
	private static Color SELECTED_END_POINT_COLOR = Color.cyan;
	private static Color SELECTED_END_POINT_CONNECTED_POINTS_COLOR = Color.green;

	private static Color AUTO_CONSTRUCT_SPLINE_BUTTON_COLOR = new Color(0.65f, 1f, 0.65f);

	private const float SPLINE_THICKNESS = 8f;
	private const float END_POINT_SIZE = 0.075f;
	private const float END_POINT_CONTROL_POINTS_SIZE = 0.05f;

	[MenuItem("GameObject/Spline", priority = 35)]
	static void NewSpline() {

		GameObject _spline = new GameObject("Spline", typeof(PD_BezierSpline));
		Undo.RegisterCreatedObjectUndo(_spline, "Create Spline");
		Selection.activeTransform = _spline.transform;
	}

	[DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable)]
	public static void DrawSplineGizmo(PD_BezierSpline _spline, GizmoType _gizmoType) {
    
		if(_spline.Count < 2) {

            return;
        }

		Gizmos.color = _spline.splineColor;

		Vector3 _lastPos = _spline[0].position;
		float _increaseAmount = (1f / (_spline.Count * SPLINE_GIZMO_SMOOTHNESS));

		for(float i = _increaseAmount; i < 1f; i += _increaseAmount) {

			Vector3 _position = _spline.GetPoint(i);
			Gizmos.DrawLine(_lastPos, _position);
			_lastPos = _position;
		}

		Gizmos.DrawLine(_lastPos, _spline.GetPoint(1f));
	}

	public static void DrawSplineDetailed(PD_BezierSpline _spline) {
    
		if(_spline.Count < 2) {

            return;
        }

		PD_BezierPoint _endPoint0 = null, endPoint1 = null;

		for(int i = 0; i < _spline.Count - 1; i++) {

			_endPoint0 = _spline[i];
			endPoint1 = _spline[i + 1];

			DrawBezier(_endPoint0, endPoint1, _spline);
		}

		if(_spline.loop && endPoint1 != null) {
        
            DrawBezier(endPoint1, _spline[0], _spline);
        }
	}

	public static void DrawSplineInspectorGUI(PD_BezierSpline _spline) {

		if(_spline.Count < 2) {

			if(GUILayout.Button( "Initialize Spline")){
            
                _spline.Reset();
            }
				
			return;
		}
		
		Color _color = GUI.color;

		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", _spline.loop);

		if(EditorGUI.EndChangeCheck()) {

			Undo.RecordObject(_spline, "Toggle Loop");
			_spline.loop = loop;

			SceneView.RepaintAll();
		}

		EditorGUI.BeginChangeCheck();
		bool drawGizmos = EditorGUILayout.Toggle("Draw Runtime Gizmos", _spline.drawGizmos);

		if(EditorGUI.EndChangeCheck()) {

			Undo.RecordObject(_spline, "Toggle Draw Gizmos");
			_spline.drawGizmos = drawGizmos;

			SceneView.RepaintAll();
		}
        
        EditorGUI.BeginChangeCheck();
        bool drawFullColor = EditorGUILayout.Toggle("Draw Full color", _spline.drawFullColor);

		if(EditorGUI.EndChangeCheck()) {

			Undo.RecordObject(_spline, "Toggle Draw Full Color");
			_spline.drawFullColor = drawFullColor;

			SceneView.RepaintAll();
		}

        EditorGUI.BeginChangeCheck();
		SplineColor _splineColor = (SplineColor) EditorGUILayout.EnumPopup("Draw Runtime Gizmos", _spline.splineColorFromEditor);

		if(EditorGUI.EndChangeCheck()) {

			Undo.RecordObject(_spline, "Change spline color");
			_spline.splineColorFromEditor = _splineColor;

			SceneView.RepaintAll();
		}
        
        EditorGUILayout.Space();

		GUI.color = AUTO_CONSTRUCT_SPLINE_BUTTON_COLOR;
        
		if(GUILayout.Button("Construct Spline from points")) {

			for(int i = 0; i < _spline.Count; i++){
            
                Undo.RecordObject(_spline[i], "Auto Construct Spline");
            }
			
			_spline.AutoConstructSpline();
			SceneView.RepaintAll();
		}
        
		GUI.color = _color;
	}

	public static void DrawBezierPoint(PD_BezierPoint _point, int _pointIndex, bool _isSelected) {

		Color _color = Handles.color;

		if(_isSelected) {

			Handles.color = SELECTED_END_POINT_COLOR;
			Handles.DotHandleCap( 0, _point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE * 1.5f, EventType.Repaint);

		} else {

			Handles.color = NORMAL_END_POINT_COLOR;

			if(Event.current.alt || Event.current.button > 0) {
            
                Handles.DotHandleCap(0, _point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE, EventType.Repaint );

            } else if(Handles.Button(_point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE, END_POINT_SIZE, Handles.DotHandleCap)){
            
                Selection.activeTransform = _point.transform;
            }
		}

		Handles.color = _color;

		Handles.DrawLine(_point.position, _point.precedingControlPointPosition);
		Handles.DrawLine(_point.position, _point.followingControlPointPosition);

		if(_isSelected == true) {
        
            Handles.color = SELECTED_END_POINT_CONNECTED_POINTS_COLOR;

        } else {
        
            Handles.color = NORMAL_END_POINT_COLOR;
        }

		Handles.RectangleHandleCap(0, _point.precedingControlPointPosition, SceneView.lastActiveSceneView.rotation, HandleUtility.GetHandleSize(_point.precedingControlPointPosition) * END_POINT_CONTROL_POINTS_SIZE, EventType.Repaint);
		Handles.RectangleHandleCap(0, _point.followingControlPointPosition, SceneView.lastActiveSceneView.rotation, HandleUtility.GetHandleSize(_point.followingControlPointPosition) * END_POINT_CONTROL_POINTS_SIZE, EventType.Repaint);

		Handles.color = _color;
		Handles.Label( _point.position, "Point" + _pointIndex );
	}

	private static void DrawBezier( PD_BezierPoint endPoint0, PD_BezierPoint endPoint1, PD_BezierSpline _spline) {

		Handles.DrawBezier( endPoint0.position, endPoint1.position, endPoint0.followingControlPointPosition, endPoint1.precedingControlPointPosition, _spline.splineColor, null, SPLINE_THICKNESS);
	}
}
