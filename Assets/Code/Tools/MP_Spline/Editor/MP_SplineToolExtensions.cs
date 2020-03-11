using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MegaPixel.Tools.Spline;

public class MP_SplineToolExtensions : EditorWindow {

	[MenuItem("CONTEXT/MP_BezierSpline/Add rigidbodies and colliders")]
	public static void DeleteRigidbodiesAndColliders(MenuCommand _command) {

		MP_BezierSpline _parent = (MP_BezierSpline) _command.context;

		for (int i = 0; i < _parent.EndPoints.Count; i++) {

			_parent.EndPoints[i].RemoveBoxTrigger();

			if (_parent.EndPoints[i].ShouldInvokeActionInCurrentPoint == true) {

				_parent.EndPoints[i].AddBoxTrigger();

			} else {

				_parent.EndPoints[i].RemoveBoxTrigger();
			}
		}
	}
}
