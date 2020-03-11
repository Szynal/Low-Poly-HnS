using UnityEditor;
using UnityEngine;

namespace MegaPixel.Tools.Spline {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(MP_FollowSpline))]
	public class MP_FollowSplineEditor : Editor {

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			MP_FollowSpline _follow = (MP_FollowSpline) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Changing progress on spline", EditorStyles.boldLabel);

			_follow.OverrideProgressOnStart = EditorGUILayout.Toggle("Override Progress On Start", _follow.OverrideProgressOnStart);
			_follow.ProgressOverrideValue = EditorGUILayout.FloatField("Progress Override Value", _follow.ProgressOverrideValue);

			EditorGUI.BeginChangeCheck();
			_follow.NewProgress = EditorGUILayout.Slider("Progress", _follow.NewProgress, 0f, 1f);

			if (EditorGUI.EndChangeCheck() == true) {

				_follow.SetProgress(_follow.NewProgress);
			}
			
			if (GUILayout.Button("Put at the begining of spline")) {
			
				_follow.SetObjectAtTheBegining();
			}

			if (_follow.spline != null) {

				if (_follow.spline.splineOwner == null) {

					_follow.spline.splineOwner = _follow;
				}
			}
		}
	}
}
