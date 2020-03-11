using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MegaPixel.Tools.Spline {

	public enum MP_SplineColor {

		green,
		red,
		blue,
		pink,
		purple,
		orange,
		cyan,
		black,
		white,
		yellow,
		gray,
		darkGreen,
		darkBlue,
		brown,
		lightBlue,
		rozowyKlaudusi,
	}

	[ExecuteInEditMode]
	public class MP_BezierSpline : MonoBehaviour {

		private static Material gizmoMaterial;

		private Color gizmoColor = Color.white;

		[Header("Spline owner")]
		public MP_FollowSpline splineOwner;

		[Header("Spline info")]
		public float SplineLenght = 0f;
		public float SplineStartTimeInSeconds = 0f;
		public float[] EndPointsPositionOnSpline = new float[] { };
		public float[] SplineSegmentsLenghts = new float[] { };
		public float[] SplineSegmentsTimers = new float[] { };
		public bool[] RotateInPoints = new bool[] { };

		float OnePointRange = 0f;
		[HideInInspector] public List<MP_BezierPoint> EndPoints = new List<MP_BezierPoint>();

		[Header("Settings")]
		public bool Loop = false;
		public bool IsPlayerSpline = false;

		[Header("Visualization")]
		public bool DrawSpline = true;
		public bool DrawFullColor = false;
		public MP_SplineColor splineColor;

		[HideInInspector] public Color splineColorForEditor = Color.green;
		[HideInInspector] public float SplineLength = 0f;

		[Space(10f)]
		public bool ShowPointsIcons = true;
		public bool ShowTimersInPoints = false;

		public int Count {

			get {

				return EndPoints.Count;
			}
		}

		public float Length {

			get {

				return GetLengthApproximately(0f, 1f);
			}
		}

		public MP_BezierPoint this[int _index] {

			get {

				if (_index < Count) {

					return EndPoints[_index];
				}

				return null;
			}
		}

		private void Awake() {

			Refresh();
		}

		private void Update() {

			setColor();
		}

		private void setColor() {

			switch (splineColor) {

				case MP_SplineColor.green:
				splineColorForEditor = Color.green;
				break;

				case MP_SplineColor.blue:
				splineColorForEditor = Color.blue;
				break;

				case MP_SplineColor.red:
				splineColorForEditor = Color.red;
				break;

				case MP_SplineColor.pink:
				splineColorForEditor = new Color(1, 0, 0.745f);
				break;

				case MP_SplineColor.purple:
				splineColorForEditor = new Color(0.2641509f, 0.02118192f, 2022177);
				break;

				case MP_SplineColor.orange:
				splineColorForEditor = new Color(1f, 0.75f, 0f);
				break;

				case MP_SplineColor.cyan:
				splineColorForEditor = Color.cyan;
				break;

				case MP_SplineColor.black:
				splineColorForEditor = new Color(0f, 0f, 0f);
				break;

				case MP_SplineColor.white:
				splineColorForEditor = new Color(1f, 1f, 1f);
				break;

				case MP_SplineColor.yellow:
				splineColorForEditor = new Color(1f, 0.97f, 0f);
				break;

				case MP_SplineColor.gray:
				splineColorForEditor = new Color(0.45f, 0.45f, 0.45f);
				break;

				case MP_SplineColor.darkBlue:
				splineColorForEditor = new Color(0.0f, 0.0f, 0.8f);
				break;

				case MP_SplineColor.darkGreen:
				splineColorForEditor = new Color(0.0f, 0.38f, 0.0f);
				break;

				case MP_SplineColor.brown:
				splineColorForEditor = new Color(0.44f, 0.22f, 0.18f);
				break;

				case MP_SplineColor.lightBlue:
				splineColorForEditor = new Color(0.3f, 0.95f, 1f);
				break;

				case MP_SplineColor.rozowyKlaudusi:
				splineColorForEditor = new Color(0.8509804f, 0.454902f, 0.5803922f);
				break;
			}
		}

#if UNITY_EDITOR

		private void OnTransformChildrenChanged() {

			Refresh();
		}
		
#endif

		void OnDrawGizmos() {

			if (DrawFullColor == true) {

				DrawSplineGizmo(this);
			}
		}
		
		public void Initialize(int _endPointsCount) {

			if (_endPointsCount < 2) {

				Debug.LogError("Not enought bezier points!");
				return;
			}

			Refresh();

			for (int i = EndPoints.Count - 1; i >= 0; i--) {

				DestroyImmediate(EndPoints[i].gameObject);
			}

			EndPoints.Clear();

			for (int i = 0; i < _endPointsCount; i++) {

				InsertNewPointAt(i);
			}

			Refresh();
		}

		public void Refresh() {

			EndPoints.Clear();
			GetComponentsInChildren(EndPoints);

			for (int i = 0; i < EndPoints.Count; i++) {

				EndPoints[i].Refresh();
			}
		}

		public void DrawSplineGizmo(MP_BezierSpline _spline) {

			if (_spline.Count < 2) {

				return;
			}

			MP_BezierPoint _endPoint0 = null, endPoint1 = null;

			for (int i = 0; i < _spline.Count - 1; i++) {

				_endPoint0 = _spline[i];
				endPoint1 = _spline[i + 1];

				DrawBezier(_endPoint0, endPoint1, splineColorForEditor);
			}
		}

		private static void DrawBezier(MP_BezierPoint endPoint0, MP_BezierPoint endPoint1, Color _color) {

			#if UNITY_EDITOR

			Handles.DrawBezier(endPoint0.position, endPoint1.position, endPoint0.followingControlPointPosition, endPoint1.precedingControlPointPosition, _color, null, 8f);

			#endif
		}

		public MP_BezierPoint InsertNewPointAt(int _index) {

			if (_index < 0 || _index > EndPoints.Count) {

				Debug.LogError("Index " + _index + " is out of range: [0," + EndPoints.Count + "]");
				return null;
			}

			int _prevCount = EndPoints.Count;

			MP_BezierPoint _point = new GameObject("Point").AddComponent<MP_BezierPoint>();
			_point.transform.SetParent(EndPoints.Count == 0 ? transform : (_index == 0 ? EndPoints[0].transform.parent : EndPoints[_index - 1].transform.parent), false);
			_point.transform.SetSiblingIndex(_index == 0 ? 0 : EndPoints[_index - 1].transform.GetSiblingIndex() + 1);

			_point.spline = GetComponentInParent<MP_BezierSpline>();

			if (EndPoints.Count == _prevCount) {

				EndPoints.Insert(_index, _point);
			}

			return _point;
		}

		public MP_BezierPoint DuplicatePointAt(int _index) {

			if (_index < 0 || _index >= EndPoints.Count) {

				Debug.LogError("Index " + _index + " is out of range: [0," + (EndPoints.Count - 1) + "]");
				return null;
			}

			MP_BezierPoint _newPoint = InsertNewPointAt(_index + 1);
			EndPoints[_index].CopyTo(_newPoint);

			return _newPoint;
		}

		public void RemovePointAt(int _index) {

			if (EndPoints.Count <= 2) {

				Debug.LogError("Can't remove point: spline must consist of at least two points!");
				return;
			}

			if (_index < 0 || _index >= EndPoints.Count) {

				Debug.LogError("Index " + _index + " is out of range: [0," + EndPoints.Count + ")");
				return;
			}

			MP_BezierPoint _point = EndPoints[_index];
			EndPoints.RemoveAt(_index);

			DestroyImmediate(_point.gameObject);
		}

		public void SwapPointsAt(int _index1, int _index2) {

			if (_index1 == _index2) {

				Debug.LogError("It's the same point!");
				return;
			}

			if (_index1 < 0 || _index1 >= EndPoints.Count || _index2 < 0 || _index2 >= EndPoints.Count) {

				Debug.LogError("Points must be in range [0," + (EndPoints.Count - 1) + "]");
				return;
			}

			MP_BezierPoint _point1 = EndPoints[_index1];
			int _point1SiblingIndex = _point1.transform.GetSiblingIndex();

			EndPoints[_index1] = EndPoints[_index2];
			EndPoints[_index2] = _point1;

			_point1.transform.SetSiblingIndex(EndPoints[_index1].transform.GetSiblingIndex());
			EndPoints[_index1].transform.SetSiblingIndex(_point1SiblingIndex);
		}

		public int IndexOf(MP_BezierPoint _point) {

			return EndPoints.IndexOf(_point);
		}

		public Vector3 GetPoint(float _normalizedT) {

			if (_normalizedT <= 0f) {

				return EndPoints[0].position;

			} else if (_normalizedT >= 1f) {

				if (Loop == true) {

					return EndPoints[0].position;
				}

				return EndPoints[EndPoints.Count - 1].position;
			}

			float _t = _normalizedT * getCount();

			MP_BezierPoint _startPoint, _endPoint;

			int _startIndex = (int) _t;
			int _endIndex = _startIndex + 1;

			if (_endIndex == EndPoints.Count) {

				_endIndex = 0;
			}

			_startPoint = EndPoints[_startIndex];
			_endPoint = EndPoints[_endIndex];

			float _localT = _t - _startIndex;
			float _oneMinusLocalT = 1f - _localT;

			return (_oneMinusLocalT * _oneMinusLocalT * _oneMinusLocalT * _startPoint.position) +
				   (3f * _oneMinusLocalT * _oneMinusLocalT * _localT * _startPoint.followingControlPointPosition) +
				   (3f * _oneMinusLocalT * _localT * _localT * _endPoint.precedingControlPointPosition) +
				   (_localT * _localT * _localT * _endPoint.position);
		}

		private int getCount() {

			int _count = 0;

			if (Loop == true) {

				_count = EndPoints.Count;

			} else {

				_count = (EndPoints.Count - 1);
			}

			return _count;
		}

		public Vector3 GetTangent(float _normalizedT) {

			if (_normalizedT <= 0f) {

				return 3f * (EndPoints[0].followingControlPointPosition - EndPoints[0].position);

			} else if (_normalizedT >= 1f) {

				if (Loop == true) {

					return 3f * (EndPoints[0].position - EndPoints[0].precedingControlPointPosition);

				} else {

					int _index = EndPoints.Count - 1;
					return 3f * (EndPoints[_index].position - EndPoints[_index].precedingControlPointPosition);
				}
			}

			float _t = _normalizedT * getCount();

			MP_BezierPoint _startPoint, _endPoint;

			int _startIndex = (int) _t;
			int _endIndex = _startIndex + 1;

			if (_endIndex == EndPoints.Count) {

				_endIndex = 0;
			}

			_startPoint = EndPoints[_startIndex];
			_endPoint = EndPoints[_endIndex];

			float _localT = _t - _startIndex;
			float _oneMinusLocalT = 1f - _localT;

			return (3f * _oneMinusLocalT * _oneMinusLocalT * (_startPoint.followingControlPointPosition - _startPoint.position)) +
				   (6f * _oneMinusLocalT * _localT * (_endPoint.precedingControlPointPosition - _startPoint.followingControlPointPosition)) +
				   (3f * _localT * _localT * (_endPoint.position - _endPoint.precedingControlPointPosition));
		}

		public float GetLengthApproximately(float _startNormalizedT, float _endNormalizedT, float _accuracy = 50f) {

			if (_endNormalizedT < _startNormalizedT) {

				float _temp = _startNormalizedT;
				_startNormalizedT = _endNormalizedT;
				_endNormalizedT = _temp;
			}

			if (_startNormalizedT < 0f) {

				_startNormalizedT = 0f;
			}

			if (_endNormalizedT > 1f) {

				_endNormalizedT = 1f;
			}

			float _step = AccuracyToStepSize(_accuracy) * (_endNormalizedT - _startNormalizedT);

			float _length = 0f;
			Vector3 _lastPoint = GetPoint(_startNormalizedT);

			for (float i = _startNormalizedT + _step; i < _endNormalizedT; i += _step) {

				Vector3 _thisPoint = GetPoint(i);
				_length += Vector3.Distance(_thisPoint, _lastPoint);
				_lastPoint = _thisPoint;
			}

			_length += Vector3.Distance(_lastPoint, GetPoint(_endNormalizedT));

			return _length;
		}

		public int GetPointIndexBasedOnProgress(float _progress) {

			for (int i = 0; i < EndPoints.Count - 1; i++) {

				if (_progress > EndPointsPositionOnSpline[i]) {

					return i;
				}
			}

			return 0;
		}

		//public void InvokeActionInPoint(int _index) {

		//	if (EndPoints[_index].ShouldInvokeActionInCurrentPoint == true) {

		//		EndPoints[_index].InvokeAction();
		//	}
		//}

		public void CalculateSplineInfo() {

			SplineLenght = Length;

			calculateEndPointsPositionsOnSpline();
			calculateSplineSegmentsLenghts();
			calculateTimersForAllPoints();
			checkIfShouldRotateInPoints();

			OnePointRange = 1f / (float) (EndPoints.Count - 1);
		}
		
		private void calculateEndPointsPositionsOnSpline() {

			EndPointsPositionOnSpline = new float[EndPoints.Count];
			float _onePointValue = 1f / (((float) EndPoints.Count) - 1f);

			EndPointsPositionOnSpline[0] = 0;
			float _iterator = 0 + _onePointValue;

			for (int i = 1; i < EndPoints.Count; i++) {

				EndPointsPositionOnSpline[i] = _iterator;
				_iterator += _onePointValue;
			}
		}

		private void calculateSplineSegmentsLenghts() {

			SplineSegmentsLenghts = new float[EndPoints.Count - 1];

			for (int i = 0; i < SplineSegmentsLenghts.Length; i++) {

				SplineSegmentsLenghts[i] = GetLengthApproximately(EndPointsPositionOnSpline[i], EndPointsPositionOnSpline[i + 1]);
			}
		}

		private void calculateTimersForAllPoints() {

			if (splineOwner == null) {

				Debug.Log("For time calculation, spline owner is needed!");
				return;
			}

			SplineSegmentsTimers = new float[EndPoints.Count];

			for (int i = 0; i < SplineSegmentsTimers.Length; i++) {

				if (i == 0) {

					SplineSegmentsTimers[i] = SplineStartTimeInSeconds;
					continue;
				}

				SplineSegmentsTimers[i] = SplineSegmentsTimers[i - 1] + (GetLengthApproximately(EndPointsPositionOnSpline[i - 1], EndPointsPositionOnSpline[i]) / splineOwner.MovementSpeed);
			}

			for (int i = 0; i < EndPoints.Count; i++) {

				if (EndPoints[i].UseCalculatedTime == true) {

					EndPoints[i].TimeInCurrentPoint = getTimerString(SplineSegmentsTimers[i]);
				}
			}
		}

		private string getTimerString(float _seconds) {

			int _min = (int) (_seconds / 60);
			int _sec = (int) (_seconds - (_min * 60));

			float _milisec = (_seconds - (_min * 60) - _sec);

			string _milisecString = "";

			if (_milisec > 0) {

				try {

					_milisecString = _milisec.ToString().Substring(2, 2);

				} catch (ArgumentOutOfRangeException _e) {

					Debug.Log(_e.ToString());
					_milisecString = "00";
				}

			} else {

				_milisecString = "00";
			}

			string _secString = "";

			if (_sec < 10) {

				_secString = "0" + _sec.ToString();

			} else {

				_secString = _sec.ToString();
			}

			return _min.ToString() + ":" + _secString + ":" + _milisecString;
		}

		private void checkIfShouldRotateInPoints() {

			RotateInPoints = new bool[EndPoints.Count];

			for (int i = 0; i < RotateInPoints.Length; i++) {

				RotateInPoints[i] = EndPoints[i].ChangeRotationInCurrentPoint;
			}
		}

		public float GetRotationValue(float _progress, int _splinePointIndex) {

			if (_splinePointIndex >= EndPoints.Count) {

				return 0;
			}

			if (_splinePointIndex <= 0) {

				return 0f;
			}

			float _degrees = 0f;
			float _progressBetweenPoints = Mathf.Clamp01(((_progress - ((_splinePointIndex - 1) * OnePointRange)) / OnePointRange));
			
			if (RotateInPoints[_splinePointIndex] == true) {

				if (RotateInPoints[_splinePointIndex - 1] == false) {

					_degrees = Mathf.Lerp(0f, EndPoints[_splinePointIndex].rotationValue, _progressBetweenPoints);

				} else {

					_degrees = Mathf.Lerp(EndPoints[_splinePointIndex - 1].rotationValue, EndPoints[_splinePointIndex].rotationValue, _progressBetweenPoints);
				}

			} else {

				if (RotateInPoints[_splinePointIndex - 1] == true) {

					if (EndPoints[_splinePointIndex - 1].splineActionType == MP_SplineActionType.resetRotationToZero) {

						_degrees = Mathf.Lerp(0, 0f, _progressBetweenPoints);

					} else {

						_degrees = Mathf.Lerp(EndPoints[_splinePointIndex - 1].rotationValue, 0f, _progressBetweenPoints);
					}
				}
			}
			
			return _degrees;
		}

		public Vector3 FindNearestPointTo(Vector3 _worldPos, float _accuracy = 100f) {

			float _normalizedT;
			return FindNearestPointTo(_worldPos, out _normalizedT, _accuracy);
		}

		public Vector3 FindNearestPointTo(Vector3 _worldPos, out float _normalizedT, float _accuracy = 100f) {

			Vector3 _result = Vector3.zero;
			_normalizedT = -1f;

			float _step = AccuracyToStepSize(_accuracy);
			float _minDistance = Mathf.Infinity;

			for (float i = 0f; i < 1f; i += _step) {

				Vector3 _thisPoint = GetPoint(i);
				float _thisDistance = (_worldPos - _thisPoint).sqrMagnitude;

				if (_thisDistance < _minDistance) {

					_minDistance = _thisDistance;
					_result = _thisPoint;
					_normalizedT = i;
				}
			}

			return _result;
		}

		public Vector3 MoveAlongSpline(ref float _normalizedT, float _deltaMovement, float _splineLength, int _accuracy = 3) {

			for (int i = 0; i < _accuracy; i++) {

				_normalizedT += _deltaMovement / (_accuracy * GetTangent(_normalizedT).magnitude);
			}

			return GetPoint(_normalizedT);
		}

		public void AutoConstructSpline() {

			for (int i = 0; i < EndPoints.Count; i++) {

				EndPoints[i].handleMode = MP_PointHandleMode.Mirrored;
			}

			int _count = EndPoints.Count - 1;

			if (_count == 1) {

				EndPoints[0].followingControlPointPosition = ((2 * EndPoints[0].position + EndPoints[1].position) / 3f);
				EndPoints[1].precedingControlPointPosition = (2 * EndPoints[0].followingControlPointPosition - EndPoints[0].position);

				return;
			}

			Vector3[] _tempPoints;

			if (Loop == true) {

				_tempPoints = new Vector3[_count + 1];

			} else {

				_tempPoints = new Vector3[_count];
			}

			for (int i = 1; i < _count - 1; i++) {

				_tempPoints[i] = 4 * EndPoints[i].position + 2 * EndPoints[i + 1].position;
			}

			_tempPoints[0] = (EndPoints[0].position + 2 * EndPoints[1].position);

			if (Loop == false) {

				_tempPoints[_count - 1] = (8 * EndPoints[_count - 1].position + EndPoints[_count].position) * 0.5f;

			} else {

				_tempPoints[_count - 1] = 4 * EndPoints[_count - 1].position + 2 * EndPoints[_count].position;
				_tempPoints[_count] = (8 * EndPoints[_count].position + EndPoints[0].position) * 0.5f;
			}

			Vector3[] _controlPoints = GetFirstControlPoints(_tempPoints);

			for (int i = 0; i < _count; i++) {

				EndPoints[i].followingControlPointPosition = _controlPoints[i];

				if (Loop == true) {

					EndPoints[i + 1].precedingControlPointPosition = 2 * EndPoints[i + 1].position - _controlPoints[i + 1];

				} else {

					if (i < _count - 1) {

						EndPoints[i + 1].precedingControlPointPosition = 2 * EndPoints[i + 1].position - _controlPoints[i + 1];

					} else {

						EndPoints[i + 1].precedingControlPointPosition = (EndPoints[_count].position + _controlPoints[_count - 1]) * 0.5f;
					}
				}
			}

			if (Loop == true) {

				float _controlPointDistance = Vector3.Distance(EndPoints[0].followingControlPointPosition, EndPoints[0].position);
				Vector3 _direction = Vector3.Normalize(EndPoints[_count].position - EndPoints[1].position);

				EndPoints[0].precedingControlPointPosition = EndPoints[0].position + _direction * _controlPointDistance * 0.5f;
				EndPoints[0].followingControlPointLocalPosition = -EndPoints[0].precedingControlPointLocalPosition;
			}
		}

		private static Vector3[] GetFirstControlPoints(Vector3[] _tempPoints) {

			int _count = _tempPoints.Length;
			Vector3[] _x = new Vector3[_count];
			float[] _temp = new float[_count];

			float _divider = 2f;
			_x[0] = _tempPoints[0] / _divider;

			for (int i = 1; i < _count; i++) {

				float _val = 1f / _divider;
				_temp[i] = _val;
				_divider = (i < _count - 1 ? 4f : 3.5f) - _val;
				_x[i] = (_tempPoints[i] - _x[i - 1]) / _divider;
			}

			for (int i = 1; i < _count; i++) {

				_x[_count - i - 1] -= _temp[_count - i] * _x[_count - i];
			}

			return _x;
		}

		private float AccuracyToStepSize(float _accuracy) {

			if (_accuracy <= 0f) {

				return 0.2f;
			}

			return Mathf.Clamp(1f / _accuracy, 0.001f, 0.2f);
		}

#if UNITY_EDITOR

		public void Reset() {

			for (int i = EndPoints.Count - 1; i >= 0; i--) {

				UnityEditor.Undo.DestroyObjectImmediate(EndPoints[i].gameObject);
			}

			Initialize(2);

			EndPoints[0].localPosition = Vector3.zero;
			EndPoints[1].localPosition = new Vector3(0f, 0f, 100f);

			UnityEditor.Undo.RegisterCreatedObjectUndo(EndPoints[0].gameObject, "Initialize Spline");
			UnityEditor.Undo.RegisterCreatedObjectUndo(EndPoints[1].gameObject, "Initialize Spline");

			UnityEditor.Selection.activeTransform = EndPoints[0].transform;
		}

		#endif
	}
}
