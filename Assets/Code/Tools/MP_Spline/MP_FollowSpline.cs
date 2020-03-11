//#define PANZER

using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using System;


namespace MegaPixel.Tools.Spline {

    [System.Serializable]
	public enum MP_SplineMovementType {

		normal,
		loop,
	}
	
	public class MP_FollowSpline: MonoBehaviour {

		Transform cachedTransform = null;
        Transform cachedChildTransform = null;
		Transform targetLookAtTransform = null;

		[Header("Debug")]
		public bool pauseGameWhenStartingMovement = false;
		public bool pauseGameWhenEndOfSpline = false;

		[Header("Spline settings")]
		public MP_BezierSpline spline;

		float splineLength = 0f;
		int endPoints = 0;
		float progress = 0f;
		int currentPointIndex = 0;

		public float Progress {

			get {

				return progress;
			}
		}

		[Header("Object behaviour settings")]
		public bool shouldBeInactiveBeforeStartOfMovement = false;
		public bool destroyGameObjectAtEndOfSpline = false;

		[Header("Main settings")]
		public MP_SplineMovementType movementType = MP_SplineMovementType.normal;

		[Space(10f)]
		public bool ShouldFollowSpline = true;
		public bool DeactivateMovementAtTheEndOfSpline = true;
		public bool KeepSplineDirection = true;
		public bool UseSplineRotation = true;
		public bool UseConstantYPosition = false;
        public bool RotateChild = false;

		[Header("Movement settings")]
		[SerializeField] float movementSpeed = 10f;
		[SerializeField] float rotationSpeed = 5f;

		[Space(10f)]
		[SerializeField] bool changeSpeedSmoothly = false;
		[SerializeField] float speedChangingValue = 0f;

		bool canSmoothlyChangeSpeed = false;
		bool shouldLookAtTarget = false;
		float targetSpeed = 0f;
		Coroutine triggersRoutine = null;

		public float MovementSpeed {

			get {

				return movementSpeed;
			}
		}

		float relaxationAtEndPoints = 0.0001f;

		[Header("End of spline event")]
		public bool isEventOneTimeOnly = true;
		public bool invokedEvent = false;
		public UnityEvent endOfSplineEvent;

		[Header("Spline progress change")]
		[HideInInspector] public bool OverrideProgressOnStart = false;
		[HideInInspector] public float ProgressOverrideValue = 0f;
		[HideInInspector] public float NewProgress = 0f;

		void Awake() {

			cachedTransform = transform;
			calculateSplineValues();

            if (RotateChild == true) {

                cachedChildTransform = transform.GetChild(0);
            }
		}

		private void calculateSplineValues() {

			if (spline != null) {

				splineLength = spline.Length;
				endPoints = spline.EndPoints.Count - 1;

				spline.CalculateSplineInfo();
			}
		}

		void Start() {

			if (OverrideProgressOnStart == false) {

				progress = 0f;

			} else {

				progress = ProgressOverrideValue;
			}

			if (shouldBeInactiveBeforeStartOfMovement == true) {

				gameObject.SetActive(false);
			}
		}

		void Update() {

			CalculateCurrentPointIndex();

			if (ShouldFollowSpline == true) {

				if (spline != null) {

					followSpline();

				} else {

					Debug.LogWarning("Please assign spline to " + gameObject.name);
				}
			}

			if (KeepSplineDirection == true || UseSplineRotation == true) {

				if (spline != null) {

					rotateToSplineDirection();

				} else {

					Debug.LogWarning("Please assign spline to " + gameObject.name);
				}
			}

			if (shouldLookAtTarget == true) {

				lookAtTarget();
			}

			if (canSmoothlyChangeSpeed == true) {

				changeMovementSpeedSmoothly();
			}
		}

		public void CalculateCurrentPointIndex() {

			if (currentPointIndex < spline.EndPoints.Count) {
			
				if (progress >= spline.EndPointsPositionOnSpline[currentPointIndex]) {

					currentPointIndex++;
					//spline.InvokeActionInPoint(currentPointIndex - 1);
					//Debug.Log("Current point index on spline: " + currentPointIndex.ToString());
				}
			}
		}

		public void SetProgress(float _newProgress) {

			cachedTransform = transform;
			progress = _newProgress;

			currentPointIndex = 0;

			for (int i = 0; i < spline.EndPoints.Count; i++) {
			
				CalculateCurrentPointIndex();
			}

			for (int i = 0; i < 30; i++) {

				cachedTransform.position = spline.MoveAlongSpline(ref progress, 0f, spline.SplineLenght);
				rotateToSplineDirection();
			}
		}

		public void ChangeRotationSpeed(float _speed) {

			rotationSpeed = _speed;
		}

		public void SetObjectAtTheBegining() {

			cachedTransform = transform;

			progress = 0;
			cachedTransform.position = spline.MoveAlongSpline(ref progress, 0, 1);
			rotateToSplineDirection();
		}

		private void followSpline() {

			float _targetSpeed;
			Vector3 _targetPosition = new Vector3();
			float _fixedSpeed = movementSpeed / endPoints;

			_targetSpeed = _fixedSpeed * Time.deltaTime;
			_targetPosition = spline.MoveAlongSpline(ref progress, _targetSpeed, splineLength);

			if (UseConstantYPosition == false) {

				cachedTransform.position = _targetPosition;

			} else {

				cachedTransform.position = new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z);
			}

			checkIfReachedEndOfTheSpline();
		}

		private void lookAtTarget() {

			Vector3 _targetDirection = targetLookAtTransform.position - transform.position;
			float _speed = 3f * Time.deltaTime;

			Quaternion _targetRotation = Quaternion.LookRotation(_targetDirection);
			transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, _speed);
		}

		private void checkIfReachedEndOfTheSpline() {

			if (progress >= 1f - relaxationAtEndPoints) {

                invokeEndOfTheSplineMethod();

                if (destroyGameObjectAtEndOfSpline == true) {
					Destroy(gameObject);
				}

				if (movementType == MP_SplineMovementType.loop) {

					progress = 0;
					return;
				}

				progress = 1f;

				if (DeactivateMovementAtTheEndOfSpline == true) {

					ShouldFollowSpline = false;
				}
				
				if (pauseGameWhenEndOfSpline == true) {

					#if UNITY_EDITOR

					EditorApplication.isPaused = true;

					#endif
				}


			} else if (progress < 0f) {

				progress = 1f - progress;
			}
		}

		private void invokeEndOfTheSplineMethod() {

			if (invokedEvent == false || isEventOneTimeOnly == false) {

				if (endOfSplineEvent != null) {

					endOfSplineEvent.Invoke();
					invokedEvent = true;
				}

				if (isEventOneTimeOnly == true) {

					endOfSplineEvent.RemoveAllListeners();
				}
			}
		}

		private void rotateToSplineDirection() {

			if (KeepSplineDirection == true) {

				Quaternion _targetRotation;
				Vector3 _rotationVector = spline.GetTangent(progress);
				_targetRotation = Quaternion.LookRotation(_rotationVector);
                
				cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
			}

			if (UseSplineRotation == true) {

                if (RotateChild == true) {

                    cachedChildTransform.localEulerAngles = new Vector3(cachedChildTransform.localEulerAngles.x, cachedChildTransform.localEulerAngles.y, spline.GetRotationValue(progress, currentPointIndex));

                } else {

                    cachedTransform.localEulerAngles = new Vector3(cachedTransform.localEulerAngles.x, cachedTransform.localEulerAngles.y, spline.GetRotationValue(progress, currentPointIndex));
                }
			}
		}

		public void ChangeSpeed(float _newSpeed) {

			if (changeSpeedSmoothly == false) {

				movementSpeed = _newSpeed;

			} else {

				targetSpeed = _newSpeed;
				canSmoothlyChangeSpeed = true;
			}

			if (shouldBeInactiveBeforeStartOfMovement == true) {

				AppearAndMove();
			}
		}

		public void ChangeSpeed(float _newSpeed, float _time) {

			float _oldSpeed = movementSpeed;

			if (changeSpeedSmoothly == false) {

				movementSpeed = _newSpeed;

			} else {

				targetSpeed = _newSpeed;
				canSmoothlyChangeSpeed = true;
			}

			cancelOldRoutine();
			triggersRoutine = StartCoroutine(goBackToOldSpeed(_oldSpeed, _time));
		}

		public void ChangeSpeedSmoothlyInTime(float _newSpeed, float _changeDuration) {
		
			StartCoroutine(smoothSpeedChange(_newSpeed, _changeDuration));
		}

		IEnumerator smoothSpeedChange(float _newSpeed, float _duration) {

			float _timer = 0;
			float _difference = Mathf.Abs(Mathf.Abs(movementSpeed) - Mathf.Abs(_newSpeed));
			
			while (_timer < _duration) {

				_timer += Time.deltaTime;
				float _changeSpeed = _difference / _duration;
				movementSpeed = Mathf.MoveTowards(movementSpeed, _newSpeed, _changeSpeed * Time.deltaTime);

				yield return null;
			}
		}

		public void StopMovement(float _speed, float _duration) {

			cancelOldRoutine();
            movementSpeed = 0f;
			triggersRoutine = StartCoroutine(goBackToOldSpeed(_speed, _duration));
		}

		private void changeMovementSpeedSmoothly() {

			float _changePerFrame = speedChangingValue * Time.deltaTime;

			if (_changePerFrame > Mathf.Abs(targetSpeed - movementSpeed)) {

				movementSpeed = targetSpeed;
				canSmoothlyChangeSpeed = false;

			} else {

				if (movementSpeed < targetSpeed) {

					movementSpeed += _changePerFrame;

				} else {

					movementSpeed -= _changePerFrame;
				}
			}
		}

		IEnumerator goBackToOldSpeed(float _speed, float _time) {

			yield return new WaitForSeconds(_time);
			ChangeSpeed(_speed);
		}

		private void cancelOldRoutine() {

			if (triggersRoutine != null) {

				StopCoroutine(triggersRoutine);
			}
		}

		public void ChangeToNextSpline(MP_BezierSpline _spline) {
		
			spline = _spline;
			progress = 0;
			invokedEvent = false;
			currentPointIndex = 0;

			ShouldFollowSpline = true;

			calculateSplineValues();
		}

		public void StartLookingAtTarget(Transform _target) {

			KeepSplineDirection = false;
			targetLookAtTransform = _target;
			shouldLookAtTarget = true;
		}

		public void StopLookingAtTarget() {

			KeepSplineDirection = true;
			shouldLookAtTarget = false;
		}

		public void StopLookingForward() {

			KeepSplineDirection = false;
		}

		public void ChangeEndSplineEvent(UnityAction _newEvent) {

			endOfSplineEvent.RemoveAllListeners();
			invokedEvent = false;
			endOfSplineEvent.AddListener(() => _newEvent());
		}

		public void ChangeSpeedParams(float _newTargetSpeed, float _newSpeedChange) {

			targetSpeed = _newTargetSpeed;
			speedChangingValue = _newSpeedChange;
			canSmoothlyChangeSpeed = true;
		}

		public IEnumerator MoveUpIndependently(float _moveSpeed, float _targetHeight, float _forwardSpeed = 0f) {

			while (transform.position.y < _targetHeight) {

				Vector3 _moveVec = new Vector3(transform.position.x, transform.position.y + (_moveSpeed * Time.deltaTime), transform.position.z);

				if (_forwardSpeed > 0f) {

					_moveVec += transform.forward * _forwardSpeed * Time.deltaTime;
				}

				transform.position = _moveVec;

				yield return null;
			}
		}

		public IEnumerator SlowDownFromSpline(float _speedChange) {

			ShouldFollowSpline = false;
			float _speed = MovementSpeed;

			while (_speed > 0f) {
				_speed -= _speedChange * Time.deltaTime;

				transform.position = transform.position + (transform.forward * (_speed * Time.deltaTime));

				yield return null;
			}
		}

		public void AppearAndMove() {

			this.enabled = true;
			gameObject.SetActive(true);

			ShouldFollowSpline = true;

            #if PANZER
            if (GetComponentInChildren<PD_Enemy>() != null) {

				GetComponentInChildren<PD_Enemy>().HandleStartingAnim();
			}
            #endif
        }
	}
}