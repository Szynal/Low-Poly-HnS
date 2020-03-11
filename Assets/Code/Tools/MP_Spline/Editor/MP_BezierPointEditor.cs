using UnityEngine;
using UnityEditor;

namespace MegaPixel.Tools.Spline {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(MP_BezierPoint))]
	public class MP_BezierPointEditor : Editor {

		private MP_BezierSpline spline;
		private MP_BezierPoint point;

		private Quaternion precedingPointRotation = Quaternion.identity;
		private Quaternion followingPointRotation = Quaternion.identity;
		private bool controlPointRotationsInitialized = false;

		void OnEnable() {

			point = target as MP_BezierPoint;
			spline = point.GetComponentInParent<MP_BezierSpline>();

			if (spline != null && !spline.Equals(null)) {

				spline.Refresh();
			}
		}

		void OnSceneGUI() {

			if (UnityEditor.Tools.current != Tool.Move) {

				controlPointRotationsInitialized = false;
				return;
			}

			if (controlPointRotationsInitialized == false) {

				precedingPointRotation = Quaternion.LookRotation(point.precedingControlPointPosition - point.position);
				followingPointRotation = Quaternion.LookRotation(point.followingControlPointPosition - point.position);

				controlPointRotationsInitialized = true;
			}

			EditorGUI.BeginChangeCheck();
			Vector3 _position = Handles.PositionHandle(point.precedingControlPointPosition, UnityEditor.Tools.pivotRotation == PivotRotation.Local ? precedingPointRotation : Quaternion.identity);

			if (EditorGUI.EndChangeCheck()) {

				Undo.RecordObject(point, "Move Control Point");
				point.precedingControlPointPosition = _position;
			}

			EditorGUI.BeginChangeCheck();
			_position = Handles.PositionHandle(point.followingControlPointPosition, UnityEditor.Tools.pivotRotation == PivotRotation.Local ? followingPointRotation : Quaternion.identity);

			if (EditorGUI.EndChangeCheck()) {

				Undo.RecordObject(point, "Move Control Point");
				point.followingControlPointPosition = _position;
			}
		}

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			if (spline.IndexOf(point) != 0 && spline.IndexOf(point) != (spline.EndPoints.Count - 1)) {

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Rotation settings", EditorStyles.boldLabel);

                point.ChangeRotationInCurrentPoint = EditorGUILayout.Toggle("Use current point rotation", point.ChangeRotationInCurrentPoint);

                if (point.ChangeRotationInCurrentPoint == true) {

                    point.currentRotationAxis = (MP_RotationAxis) EditorGUILayout.EnumPopup("Current rotation axis", point.currentRotationAxis);
                    point.rotationValue = EditorGUILayout.FloatField("Rotation value", point.rotationValue);
                }

                EditorGUILayout.Space();
				EditorGUILayout.LabelField("Action in current point", EditorStyles.boldLabel);

				EditorGUI.BeginChangeCheck();
				point.ShouldInvokeActionInCurrentPoint = EditorGUILayout.Toggle("Should Invoke Action In Current Point", point.ShouldInvokeActionInCurrentPoint);

				if (EditorGUI.EndChangeCheck() == true) {

					if (point.ShouldInvokeActionInCurrentPoint == true) {

						point.AddBoxTrigger();

					} else {

						point.RemoveBoxTrigger();
					}
				}

				if (point.ShouldInvokeActionInCurrentPoint == true) {

					point.splineActionType = (MP_SplineActionType) EditorGUILayout.EnumPopup("Action type", point.splineActionType);

					switch (point.splineActionType) {

						case MP_SplineActionType.simpleChangeSpeed:
						point.newSpeed = EditorGUILayout.FloatField("New speed", point.newSpeed);
						break;
						
						case MP_SplineActionType.stopMovement:
						point.stopDuration = EditorGUILayout.FloatField("Stop duration", point.stopDuration);
						break;

						case MP_SplineActionType.startLookingAtTarget:
						point.lookAtTarget = EditorGUILayout.ObjectField("Look at target", point.lookAtTarget, typeof(Transform), true) as Transform;

						if (GUILayout.Button("Assign player")) {

							point.lookAtTarget = GameObject.FindGameObjectWithTag("Player").transform;
						}

						break;

						case MP_SplineActionType.stopLookingAtTarget:
						//Nothing here but space :D 
						break;

						case MP_SplineActionType.customEvent:

						SerializedProperty _serializedProp = serializedObject.FindProperty("customEvent");

						EditorGUILayout.PropertyField(_serializedProp);
						serializedObject.ApplyModifiedProperties();
						break;

						case MP_SplineActionType.attachSplineToObject:
						point.attachTarget = EditorGUILayout.ObjectField("Attach target", point.attachTarget, typeof(Transform), true) as Transform;

						if (GUILayout.Button("Assign player controller")) {

							point.attachTarget = GameObject.FindGameObjectWithTag("Player").transform.parent;
						}

						break;

						case MP_SplineActionType.changeSpeedSmoothly:
						point.newSpeed = EditorGUILayout.FloatField("New Speed", point.newSpeed);
						point.smoothSpeedChangeDuration = EditorGUILayout.FloatField("Smooth speed change duration", point.smoothSpeedChangeDuration);
						break;

						case MP_SplineActionType.changeRotationSpeed:
						point.newRotationSpeed = EditorGUILayout.FloatField("New rotation speed", point.newRotationSpeed);
						break;

						case MP_SplineActionType.startAnimation:
						point.animationString = EditorGUILayout.TextField("Trigger string", point.animationString);
						point.animatorController = EditorGUILayout.ObjectField("Animator", point.animatorController, typeof(Animator), true) as Animator;
						break;

						case MP_SplineActionType.changeSpeedForGivenTime:
						point.newSpeed = EditorGUILayout.FloatField("New Speed", point.newSpeed);
						point.speedChangeDuration = EditorGUILayout.FloatField("Speed change duration", point.speedChangeDuration);
						break;
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Point settings", EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();
			MP_PointHandleMode handleMode = (MP_PointHandleMode) EditorGUILayout.EnumPopup("Handle Mode", point.handleMode);

			if (EditorGUI.EndChangeCheck()) {

				Undo.RecordObject(point, "Change Point Handle Mode");
				point.handleMode = handleMode;

				SceneView.RepaintAll();
			}

			EditorGUILayout.Space();

			if (spline != null && !spline.Equals(null)) {

				if (point == null || point.Equals(null)) {

					return;
				}

				EditorGUILayout.LabelField("Extending spline", EditorStyles.boldLabel);

				if (GUILayout.Button("Insert Point Before")) {

					InsertNewPointAt(spline.IndexOf(point));
				}

				if (GUILayout.Button("Insert Point After")) {

					InsertNewPointAt(spline.IndexOf(point) + 1);
				}
			}

			EditorGUILayout.Space();

			if (spline != null && !spline.Equals(null)) {

				EditorGUILayout.LabelField("Managing current point", EditorStyles.boldLabel);

				if (GUILayout.Button("Reset Point")) {

					ResetEndPointAt(spline.IndexOf(point));
					SceneView.RepaintAll();
				}

				if (spline.Count <= 2) {

					GUI.enabled = false;
				}

				if (GUILayout.Button("Remove Point")) {

					RemovePointAt(spline.IndexOf(point));
				}
			}

			GUI.enabled = true;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);

			if (GUILayout.Button("Select spline")) {

				Selection.activeObject = point.gameObject.transform.parent.gameObject;
			}
		}

		private void InsertNewPointAt(int _index) {

			Vector3 _position;

			if (spline.Count >= 2) {

				if (_index > 0 && _index < spline.Count) {

					_position = (spline[_index - 1].localPosition + spline[_index].localPosition) * 0.5f;

				} else if (_index == 0) {

					if (spline.Loop == true) {

						_position = (spline[0].localPosition + spline[spline.Count - 1].localPosition) * 0.5f;

					} else {

						_position = spline[0].localPosition - (spline[1].localPosition - spline[0].localPosition) * 0.5f;
					}

				} else {

					if (spline.Loop == true) {

						_position = (spline[0].localPosition + spline[spline.Count - 1].localPosition) * 0.5f;

					} else {

						_position = spline[_index - 1].localPosition + (spline[_index - 1].localPosition - spline[_index - 2].localPosition) * 0.5f;
					}
				}

			} else if (spline.Count == 1) {

				if (_index == 0) {

					_position = spline[0].localPosition - Vector3.forward;

				} else {

					_position = spline[0].localPosition + Vector3.forward;
				}

			} else {

				_position = Vector3.zero;
			}

			MP_BezierPoint _point = spline.InsertNewPointAt(_index);
			_point.localPosition = _position;

			Undo.IncrementCurrentGroup();
			Undo.RegisterCreatedObjectUndo(_point.gameObject, "Insert Point");
			Undo.RegisterCompleteObjectUndo(_point.transform.parent, "Insert Point");

			Selection.activeTransform = _point.transform;
			SceneView.RepaintAll();
		}

		private void RemovePointAt(int _index) {

			if (spline.Count <= 2) {

				return;
			}

			Undo.IncrementCurrentGroup();
			Undo.DestroyObjectImmediate(spline[_index].gameObject);

			if (_index >= spline.Count) {

				_index--;
			}

			Selection.activeTransform = spline[_index].transform;
			SceneView.RepaintAll();
		}

		private void ResetEndPointAt(int _index) {

			Undo.RecordObject(spline[_index].transform, "Reset Point");
			Undo.RecordObject(spline[_index], "Reset Point");

			spline[_index].Reset();
		}

		private void OnUndoRedo() {

			controlPointRotationsInitialized = false;

			if (spline != null && !spline.Equals(null)) {

				spline.Refresh();
			}

			Repaint();
		}

		private bool HasFrameBounds() {

			return true;
		}

		private Bounds OnGetFrameBounds() {

			return new Bounds(point.position, Vector3.one);
		}
	}
}