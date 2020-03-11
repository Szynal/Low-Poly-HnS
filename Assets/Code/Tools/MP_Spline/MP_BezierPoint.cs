using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace MegaPixel.Tools.Spline {

    [System.Serializable]
	public enum MP_PointHandleMode {

		Free,
		Aligned,
		Mirrored
	};

    [System.Serializable]
    public enum MP_RotationAxis {

        localX,
        localY,
        localZ,
    }

	[System.Serializable]
	public enum MP_SplineActionType {

		simpleChangeSpeed,
		changeRotationSpeed,
		stopMovement,
		startLookingAtTarget,
		stopLookingAtTarget,
		customEvent,
		attachSplineToObject,
		detachSpline,
		resetRotationToZero,
		changeSpeedSmoothly,
		startKeepingSplineDirection,
		stopKeepingSplineDirection,
		startAnimation,
		changeSpeedForGivenTime,
	}
	
	public class MP_BezierPoint : MonoBehaviour {

		[HideInInspector] public MP_BezierSpline spline;

		[Header("Debug")]
		public bool PauseInCurrentPoint = false;
		public bool PrintObjectsWhichTouchedTrigger = false;

		[Header("Timing settings")]
		public bool ShowTimeInCurrentPoint = true;
		public bool UseCalculatedTime = true;
		public string TimeInCurrentPoint = "0:00:00";

        [Header("Rotation settings")]
        [HideInInspector] public bool ChangeRotationInCurrentPoint = false;
        [HideInInspector] public MP_RotationAxis currentRotationAxis = MP_RotationAxis.localZ;
        [HideInInspector] public float rotationValue = 0f;

		[Header("Invoke action in current point")]
		[HideInInspector] public bool ShouldInvokeActionInCurrentPoint = false;
		[HideInInspector] public MP_SplineActionType splineActionType = MP_SplineActionType.simpleChangeSpeed;
		[HideInInspector] public float newSpeed = 0f;
		[HideInInspector] public float stopDuration = 0f;
		[HideInInspector] public float speedChangeDuration = 0f;
		[HideInInspector] public Transform lookAtTarget = null;
		[HideInInspector] public UnityEvent customEvent;
		[HideInInspector] public Transform attachTarget = null;
		[HideInInspector] public float smoothSpeedChangeDuration = 0f;
		[HideInInspector] public float newRotationSpeed = 1f;
		[HideInInspector] public string animationString = "";
		[HideInInspector] public Animator animatorController = null;
		
		public Vector3 localPosition {

			get {

				return transform.localPosition;
			}

			set {

				transform.localPosition = value;
			}
		}

		[SerializeField]
		[HideInInspector] private Vector3 m_position;

		public Vector3 position {

			get {

				if (transform.hasChanged) {

					Revalidate();
				}

				return m_position;
			}

			set {

				transform.position = value;
			}
		}
		
		[SerializeField]
		[HideInInspector] private Vector3 m_precedingControlPointLocalPosition = Vector3.left;

		public Vector3 precedingControlPointLocalPosition {

			get {

				return m_precedingControlPointLocalPosition;
			}

			set {

				m_precedingControlPointLocalPosition = value;
				m_precedingControlPointPosition = transform.TransformPoint(value);

				if (m_handleMode == MP_PointHandleMode.Aligned) {

					m_followingControlPointLocalPosition = -m_precedingControlPointLocalPosition.normalized * m_followingControlPointLocalPosition.magnitude;
					m_followingControlPointPosition = transform.TransformPoint(m_followingControlPointLocalPosition);

				} else if (m_handleMode == MP_PointHandleMode.Mirrored) {

					m_followingControlPointLocalPosition = -m_precedingControlPointLocalPosition;
					m_followingControlPointPosition = transform.TransformPoint(m_followingControlPointLocalPosition);
				}
			}
		}

		[SerializeField]
		[HideInInspector] private Vector3 m_precedingControlPointPosition;

		public Vector3 precedingControlPointPosition {

			get {

				if (transform.hasChanged) {

					Revalidate();
				}

				return m_precedingControlPointPosition;
			}

			set {

				m_precedingControlPointPosition = value;
				m_precedingControlPointLocalPosition = transform.InverseTransformPoint(value);

				if (transform.hasChanged) {

					m_position = transform.position;
					transform.hasChanged = false;
				}

				if (m_handleMode == MP_PointHandleMode.Aligned) {

					m_followingControlPointPosition = m_position - (m_precedingControlPointPosition - m_position).normalized * (m_followingControlPointPosition - m_position).magnitude;
					m_followingControlPointLocalPosition = transform.InverseTransformPoint(m_followingControlPointPosition);

				} else if (m_handleMode == MP_PointHandleMode.Mirrored) {

					m_followingControlPointPosition = 2f * m_position - m_precedingControlPointPosition;
					m_followingControlPointLocalPosition = transform.InverseTransformPoint(m_followingControlPointPosition);
				}
			}
		}

		[SerializeField]
		[HideInInspector] private Vector3 m_followingControlPointLocalPosition = Vector3.right;

		public Vector3 followingControlPointLocalPosition {

			get {

				return m_followingControlPointLocalPosition;
			}

			set {

				m_followingControlPointLocalPosition = value;
				m_followingControlPointPosition = transform.TransformPoint(value);

				if (m_handleMode == MP_PointHandleMode.Aligned) {

					m_precedingControlPointLocalPosition = -m_followingControlPointLocalPosition.normalized * m_precedingControlPointLocalPosition.magnitude;
					m_precedingControlPointPosition = transform.TransformPoint(m_precedingControlPointLocalPosition);
				} else if (m_handleMode == MP_PointHandleMode.Mirrored) {

					m_precedingControlPointLocalPosition = -m_followingControlPointLocalPosition;
					m_precedingControlPointPosition = transform.TransformPoint(m_precedingControlPointLocalPosition);
				}
			}
		}

		[SerializeField]
		[HideInInspector] private Vector3 m_followingControlPointPosition;

		public Vector3 followingControlPointPosition {

			get {

				if (transform.hasChanged) {

					Revalidate();
				}

				return m_followingControlPointPosition;
			}

			set {

				m_followingControlPointPosition = value;
				m_followingControlPointLocalPosition = transform.InverseTransformPoint(value);

				if (transform.hasChanged) {

					m_position = transform.position;
					transform.hasChanged = false;
				}

				if (m_handleMode == MP_PointHandleMode.Aligned) {

					m_precedingControlPointPosition = m_position - (m_followingControlPointPosition - m_position).normalized * (m_precedingControlPointPosition - m_position).magnitude;
					m_precedingControlPointLocalPosition = transform.InverseTransformPoint(m_precedingControlPointPosition);

				} else if (m_handleMode == MP_PointHandleMode.Mirrored) {

					m_precedingControlPointPosition = 2f * m_position - m_followingControlPointPosition;
					m_precedingControlPointLocalPosition = transform.InverseTransformPoint(m_precedingControlPointPosition);
				}
			}
		}

		[SerializeField]
		[HideInInspector] private MP_PointHandleMode m_handleMode = MP_PointHandleMode.Mirrored;

		public MP_PointHandleMode handleMode {

			get {

				return m_handleMode;
			}

			set {

				m_handleMode = value;

				if (value == MP_PointHandleMode.Aligned || value == MP_PointHandleMode.Mirrored) {

					precedingControlPointLocalPosition = m_precedingControlPointLocalPosition;
				}
			}
		}

		private void Awake() {

			transform.hasChanged = true;
			Refresh();
		}
		
		public void Refresh() {

			spline = GetComponentInParent<MP_BezierSpline>();
		}

		private void OnTriggerEnter(Collider _other) {
		
			bool _otherIsSame = false;
			_otherIsSame = GameObject.ReferenceEquals(_other.gameObject, spline.splineOwner.gameObject);
			
			if (PrintObjectsWhichTouchedTrigger == true) {

				Debug.Log("PointName: " + gameObject.name + ", SplineName: " + spline.gameObject.name + ", Instigator: " + _other.name + ", Spline owner: " + spline.splineOwner.gameObject.name + ". They are same: " + _otherIsSame);
			}

			if (_otherIsSame == true) {
			
				#if UNITY_EDITOR

				if (PauseInCurrentPoint == true) {

					EditorApplication.isPaused = true;
				}

				#endif
				
				switch (splineActionType) {

					case MP_SplineActionType.simpleChangeSpeed:
					spline.splineOwner.ChangeSpeed(newSpeed);
					break;

					case MP_SplineActionType.stopMovement:
					spline.splineOwner.StopMovement(spline.splineOwner.MovementSpeed, stopDuration);
					break;

					case MP_SplineActionType.changeRotationSpeed:
					spline.splineOwner.ChangeRotationSpeed(newRotationSpeed);
					break;

					case MP_SplineActionType.startLookingAtTarget:
					spline.splineOwner.StartLookingAtTarget(lookAtTarget);
					break;

					case MP_SplineActionType.stopLookingAtTarget:
					spline.splineOwner.StopLookingAtTarget();
					break;

					case MP_SplineActionType.customEvent:

					if (customEvent != null) {

						customEvent.Invoke();
					//	customEvent = null;
					}

					break;

					case MP_SplineActionType.attachSplineToObject:
					spline.transform.parent = attachTarget;
					break;

					case MP_SplineActionType.detachSpline:
					spline.transform.parent = null;
					break;

					case MP_SplineActionType.resetRotationToZero:
					spline.splineOwner.transform.localEulerAngles = new Vector3(spline.splineOwner.transform.localEulerAngles.x, spline.splineOwner.transform.localEulerAngles.y, 0f);
					break;

					case MP_SplineActionType.changeSpeedSmoothly:
					spline.splineOwner.ChangeSpeedSmoothlyInTime(newSpeed, smoothSpeedChangeDuration);
					break;

					case MP_SplineActionType.startKeepingSplineDirection:
					spline.splineOwner.KeepSplineDirection = true;
					break;

					case MP_SplineActionType.stopKeepingSplineDirection:
					spline.splineOwner.KeepSplineDirection = false;
					break;

					case MP_SplineActionType.startAnimation:

					if (animatorController != null) {

						animatorController.SetTrigger(animationString);
					}
					
					break;

					case MP_SplineActionType.changeSpeedForGivenTime:
					spline.splineOwner.ChangeSpeed(newSpeed, speedChangeDuration);
					break;
				}

				//GetComponentInChildren<BoxCollider>().enabled = false;
			}
		}

		public void AddBoxTrigger() {

			RemoveBoxTrigger();

			BoxCollider _box = new GameObject("Trigger").AddComponent<BoxCollider>();

			_box.isTrigger = true;
			_box.center = new Vector3(0, 0, 2.5f);
			

			if (spline.IsPlayerSpline == true) {

				_box.gameObject.layer = 22;
				_box.size = new Vector3(45f, 25f, 5f);

			} else {

				_box.gameObject.layer = 21;
				_box.size = new Vector3(5f, 5f, 5f);
			}
			
			_box.transform.parent = transform;
			_box.transform.localPosition = Vector3.zero;

			Rigidbody _rb = gameObject.AddComponent<Rigidbody>();
			
			_rb.useGravity = false;
			_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}

		public void RemoveBoxTrigger() {

			if (transform.childCount > 0) {

				DestroyImmediate(transform.GetChild(0).gameObject);
			}

			if (gameObject.GetComponent<Rigidbody>() != null) {

				DestroyImmediate(gameObject.GetComponent<Rigidbody>());
			}
		}

		public void Reset() {

			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;

			precedingControlPointLocalPosition = Vector3.left;
			followingControlPointLocalPosition = Vector3.right;

			transform.hasChanged = true;
		}

		public void CopyTo(MP_BezierPoint _other) {

			_other.transform.localPosition = transform.localPosition;
			_other.transform.localRotation = transform.localRotation;
			_other.transform.localScale = transform.localScale;

			_other.m_handleMode = m_handleMode;
			_other.m_precedingControlPointLocalPosition = m_precedingControlPointLocalPosition;
			_other.m_followingControlPointLocalPosition = m_followingControlPointLocalPosition;
		}

		private void Revalidate() {

			m_position = transform.position;
			m_precedingControlPointPosition = transform.TransformPoint(m_precedingControlPointLocalPosition);
			m_followingControlPointPosition = transform.TransformPoint(m_followingControlPointLocalPosition);

			transform.hasChanged = false;
		}

		public static Vector3 GetPoint(float _t, Vector3 _startPoint, Vector3 _endPoint, Vector3 _controlPoint) {

			float _OneMinutT = 1 - _t;
			float _double = _OneMinutT * _OneMinutT;

			Vector3 _point = _double * _startPoint;
			_point += 2 * _OneMinutT * _t * _controlPoint;
			_point += _t * _t * _endPoint;

			return _point;
		}

        #if UNITY_EDITOR

        private void OnDrawGizmos() {

			if (spline == null || spline.DrawSpline == false) {

				return;
			}

			if (spline.ShowPointsIcons == true) {

                if (ChangeRotationInCurrentPoint == true && ShouldInvokeActionInCurrentPoint == true) {

                    Gizmos.DrawIcon(transform.position, "spline_icon_actionRotation", false);

                } else if (ChangeRotationInCurrentPoint == true) {

                    Gizmos.DrawIcon(transform.position, "spline_icon_rotation", false);

                } else if (ShouldInvokeActionInCurrentPoint == true) {

                    Gizmos.DrawIcon(transform.position, "spline_icon_action", false);

                } else {

                    Gizmos.DrawIcon(transform.position, "spline_icon_normal", false);
                }
			}

			if (spline.ShowTimersInPoints == true && ShowTimeInCurrentPoint == true) {

				GUIStyle _style = new GUIStyle();

				if (ShouldInvokeActionInCurrentPoint == true && ChangeRotationInCurrentPoint == true) {

					_style.normal.textColor = Color.magenta;

				} else if (ChangeRotationInCurrentPoint == true) {

					_style.normal.textColor = Color.blue;

				} else if (ShouldInvokeActionInCurrentPoint == true) {

					_style.normal.textColor = Color.red;

				} else {

					_style.normal.textColor = Color.black;
				}
				

				_style.fontSize = 15;
				_style.fontStyle = FontStyle.Bold;
				_style.alignment = TextAnchor.MiddleCenter;

				Handles.Label(transform.position + new Vector3(0f, 0.5f, 0f), TimeInCurrentPoint, _style);
			}
		}

        #endif
    }
}