using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MegaPixel.Tools.Spline;

public class PD_ConvertOldSplinesToNew : MonoBehaviour {

	[MenuItem("CONTEXT/PD_BezierSpline/Convert to new system")]
	public static void Convert(MenuCommand _command) {

		PD_BezierSpline _oldSpline = (PD_BezierSpline) _command.context;

		if (_oldSpline != null) {

			GameObject _newSpline = new GameObject(_oldSpline.name + "_new") as GameObject;

			_newSpline.AddComponent<MP_BezierSpline>();
			_newSpline.transform.parent = _oldSpline.transform.parent;

			_newSpline.transform.localPosition = _oldSpline.transform.localPosition;
			_newSpline.transform.localRotation = _oldSpline.transform.localRotation;
			_newSpline.transform.localScale = _oldSpline.transform.localScale;

			MP_BezierSpline _spline = _newSpline.GetComponent<MP_BezierSpline>();

			for (int i = 0; i < _oldSpline.endPoints.Count; i++) {

				if (_spline.EndPoints.Count < _oldSpline.endPoints.Count) {

					_spline.InsertNewPointAt(0);
				}
			}

			for (int i = 0; i < _spline.EndPoints.Count; i++) {

				_spline.EndPoints[i].localPosition = _oldSpline.endPoints[i].localPosition;
				_spline.EndPoints[i].precedingControlPointLocalPosition = _oldSpline.endPoints[i].precedingControlPointLocalPosition;
				_spline.EndPoints[i].followingControlPointLocalPosition = _oldSpline.endPoints[i].followingControlPointLocalPosition;
			}

			List<PD_FollowSpline> _objsToSelect = new List<PD_FollowSpline>();
			PD_FollowSpline[] _foundObjs = Resources.FindObjectsOfTypeAll<PD_FollowSpline>();

			foreach (PD_FollowSpline _flwSpline in _foundObjs) {

				if (_flwSpline.gameObject.scene.isLoaded == true) {

					if (_flwSpline.spline == _oldSpline) {

						_objsToSelect.Add(_flwSpline);
					}
				}
			}

			Selection.objects = _objsToSelect.ToArray();
			_oldSpline.gameObject.SetActive(false);

		} else {

			Debug.LogError("No spline attached!");
		}
	}
}
