using UnityEngine;
using UnityEditor;

namespace MegaPixel.Tools.Spline {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(MP_BezierSpline))]
	public class MP_BezierSplineEditor : Editor {

		private MP_BezierSpline spline;

		void OnEnable() {

			spline = target as MP_BezierSpline;
			spline.Refresh();

			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		void OnDisable() {

			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		void OnSceneGUI() {

			if (spline != null && spline.DrawSpline == true) {

				MP_BezierUtils.DrawSplineDetailed(spline);

				for (int i = 0; i < spline.Count; i++) {

					MP_BezierUtils.DrawBezierPoint(spline[i], i + 1, false);
				}
			}
		}

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			MP_BezierSpline _spline = (MP_BezierSpline) target;

			if (_spline.Count < 2) {

				if (GUILayout.Button("Initialize Spline")) {

					_spline.Reset();
				}

				return;
			}

			Color _color = GUI.color;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);

			GUI.color = Color.green;

			if (GUILayout.Button("Smooth spline")) {

				for (int i = 0; i < _spline.Count; i++) {

					Undo.RecordObject(_spline[i], "Auto Construct Spline");
				}

				_spline.AutoConstructSpline();
				SceneView.RepaintAll();
			}

			GUI.color = _color;

			if (GUILayout.Button("Recalculate spline info and timers")) {

				spline.CalculateSplineInfo();
			}
		}

		private void OnUndoRedo() {

			if (spline != null && !spline.Equals(null)) {

				spline.Refresh();
			}

			Repaint();
		}

		private bool HasFrameBounds() {

			return true;
		}

		private Bounds OnGetFrameBounds() {

			return new Bounds(spline.transform.position, Vector3.one);
		}
	}
}