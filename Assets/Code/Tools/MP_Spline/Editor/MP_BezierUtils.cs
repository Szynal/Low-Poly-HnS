using UnityEngine;
using UnityEditor;

namespace MegaPixel.Tools.Spline {

	public static class MP_BezierUtils {

		private const int SPLINE_GIZMO_SMOOTHNESS = 10;

		private static Color NORMAL_END_POINT_COLOR = new Color(0.22f, 0.88f, 0.9f);
		private static Color SELECTED_END_POINT_COLOR = Color.cyan;
		private static Color SELECTED_END_POINT_CONNECTED_POINTS_COLOR = Color.green;

		private static Color AUTO_CONSTRUCT_SPLINE_BUTTON_COLOR = new Color(0.65f, 1f, 0.65f);

		private const float SPLINE_THICKNESS = 8f;
		private const float END_POINT_SIZE = 0.075f;
		private const float END_POINT_CONTROL_POINTS_SIZE = 0.05f;

		[MenuItem("GameObject/MP Utility/Spline", priority = 40)]
		static void NewSpline() {

			GameObject _spline = new GameObject("MP_Spline", typeof(MP_BezierSpline));
			Undo.RegisterCreatedObjectUndo(_spline, "Create Spline");
			Selection.activeTransform = _spline.transform;
		}

		[DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable)]
		public static void DrawSplineGizmo(MP_BezierSpline _spline, GizmoType _gizmoType) {

			if (_spline.Count < 2) {

				return;
			}

			Gizmos.color = _spline.splineColorForEditor;

			Vector3 _lastPos = _spline[0].position;
			float _increaseAmount = (1f / (_spline.Count * SPLINE_GIZMO_SMOOTHNESS));

			for (float i = _increaseAmount; i < 1f; i += _increaseAmount) {

				Vector3 _position = _spline.GetPoint(i);
				Gizmos.DrawLine(_lastPos, _position);
				_lastPos = _position;
			}

			Gizmos.DrawLine(_lastPos, _spline.GetPoint(1f));
		}

		public static void DrawSplineDetailed(MP_BezierSpline _spline) {

			if (_spline.Count < 2) {

				return;
			}

			MP_BezierPoint _endPoint0 = null, endPoint1 = null;

			for (int i = 0; i < _spline.Count - 1; i++) {

				_endPoint0 = _spline[i];
				endPoint1 = _spline[i + 1];

				DrawBezier(_endPoint0, endPoint1, _spline);
			}

			if (_spline.Loop && endPoint1 != null) {

				DrawBezier(endPoint1, _spline[0], _spline);
			}
		}

		public static void DrawBezierPoint(MP_BezierPoint _point, int _pointIndex, bool _isSelected) {

			Color _color = Handles.color;

			if (_isSelected) {

				Handles.color = SELECTED_END_POINT_COLOR;
				Handles.DotHandleCap(0, _point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE * 1.5f, EventType.Repaint);

			} else {

				Handles.color = NORMAL_END_POINT_COLOR;

				if (Event.current.alt || Event.current.button > 0) {

					Handles.DotHandleCap(0, _point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE, EventType.Repaint);

				} else if (Handles.Button(_point.position, Quaternion.identity, HandleUtility.GetHandleSize(_point.position) * END_POINT_SIZE, END_POINT_SIZE, Handles.DotHandleCap)) {

					Selection.activeTransform = _point.transform;
				}
			}

			Handles.color = _color;

			Handles.DrawLine(_point.position, _point.precedingControlPointPosition);
			Handles.DrawLine(_point.position, _point.followingControlPointPosition);

			if (_isSelected == true) {

				Handles.color = SELECTED_END_POINT_CONNECTED_POINTS_COLOR;

			} else {

				Handles.color = NORMAL_END_POINT_COLOR;
			}

			Handles.RectangleHandleCap(0, _point.precedingControlPointPosition, SceneView.lastActiveSceneView.rotation, HandleUtility.GetHandleSize(_point.precedingControlPointPosition) * END_POINT_CONTROL_POINTS_SIZE, EventType.Repaint);
			Handles.RectangleHandleCap(0, _point.followingControlPointPosition, SceneView.lastActiveSceneView.rotation, HandleUtility.GetHandleSize(_point.followingControlPointPosition) * END_POINT_CONTROL_POINTS_SIZE, EventType.Repaint);

			Handles.color = _color;
			//Handles.Label( _point.position, "Point" + _pointIndex );
		}

		private static void DrawBezier(MP_BezierPoint endPoint0, MP_BezierPoint endPoint1, MP_BezierSpline _spline) {

			Handles.DrawBezier(endPoint0.position, endPoint1.position, endPoint0.followingControlPointPosition, endPoint1.precedingControlPointPosition, _spline.splineColorForEditor, null, SPLINE_THICKNESS);
		}
	}
}