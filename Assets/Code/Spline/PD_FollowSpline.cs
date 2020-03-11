using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PD_FollowSpline: MonoBehaviour {

    Transform cachedTransform;

    [Header("Spline to attach")]
    public PD_BezierSpline spline;

    [Header("Movement")]
    public bool shouldFollowSpline = true;
    [SerializeField] float movementSpeed;

    public float MovementSpeed { get { return movementSpeed; } }

    [SerializeField] bool shouldStartAfterSomeTime = false;
    [SerializeField] float startDelay = 0f;
    [SerializeField] bool destroyAtEndOfSpline = false;
    
    [Space(10)]
    [SerializeField] bool changeSpeedSmoothly = false;
    [SerializeField] float speedChangingSpeed;

    float targetSpeed;
    [SerializeField] bool canChangeSpeed = false;
	float progress = 0f;
    
    [Header("Rotations")]
	[Range( 0f, 0.06f )]
	[SerializeField] float relaxationAtEndPoints = 0.001f;
	[SerializeField] float rotationLerpModifier = 10f;

	public bool lookForward = true;

    [Header("End of movement")]
    public bool isEventOneTimeOnly = true;
    public bool invokedEvent = false;
    public UnityEvent endOfSplineEvent;
    
    void Awake() {

		cachedTransform = transform;
	}

    void Start() {
    
        if (shouldStartAfterSomeTime == true) {

            StartCoroutine(startAfterSomeTime());
        }
    }

    IEnumerator startAfterSomeTime() {

        yield return new WaitForSeconds(startDelay);
        shouldFollowSpline = true;
    }

	void Update() {

        if(shouldFollowSpline == true) {

            if (spline != null) {

                followSpline();

            } else {

                //Debug.LogError("Please assign spline to " + gameObject.name);
            }
        }
        
        if (lookForward == true) {
        
            if (spline != null) {

                rotateToSplineDirection();

            } else {

                //Debug.LogError("Please assign spline to " + gameObject.name);
            }
        }

        if (canChangeSpeed == true) {

            changeMovementSpeedSmoothly();
        }
    }
    
    public void SetProgress(float _new) {

        progress = _new;
    }

    private void followSpline() {

        float _targetSpeed = movementSpeed * Time.deltaTime;
        Vector3 _targetPosition = spline.MoveAlongSpline(ref progress, _targetSpeed);

        cachedTransform.position = _targetPosition;
        checkIfReachedEndOfTheSpline();
    }

    private void checkIfReachedEndOfTheSpline() {

        if (progress >= 1f - relaxationAtEndPoints) {

            progress = 1f;
            
            if (destroyAtEndOfSpline == true) {

                shouldFollowSpline = false;
                invokeEndOfTheSplineMethod();
                Destroy(this);
            }

            invokeEndOfTheSplineMethod();
        }
    }

    private void invokeEndOfTheSplineMethod() {

        if (invokedEvent == false) {
            
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

        if (lookForward == true) {

            Quaternion _targetRotation;
            _targetRotation = Quaternion.LookRotation(spline.GetTangent(progress));

            cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, _targetRotation, rotationLerpModifier * Time.deltaTime);
        }
    }
    
    public void ChangeSpeed(float _newSpeed) {

        if (changeSpeedSmoothly == false) {

            movementSpeed = _newSpeed;

        } else {

            targetSpeed = _newSpeed;
            canChangeSpeed = true;
        }
    }

    public void ChangeSpeed(float _newSpeed, float _time) {

        float _oldSpeed = movementSpeed;

        if (changeSpeedSmoothly == false) {
        
            movementSpeed = _newSpeed;

        } else {

            targetSpeed = _newSpeed;
            canChangeSpeed = true;
        }
        
        StartCoroutine(goBackToOldSpeed(_oldSpeed, _time));
    }

    private void changeMovementSpeedSmoothly() {

        float _changePerFrame = speedChangingSpeed * Time.deltaTime;
        

        if (_changePerFrame > Mathf.Abs(targetSpeed - movementSpeed)) {
        
            movementSpeed = targetSpeed;
            canChangeSpeed = false;

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

    public void ChangeToNextSpline(PD_BezierSpline _spline) {

        spline = _spline;
        progress = 0;
        invokedEvent = false;
    }

    public void StopLookingForward() {

        lookForward = false;
    }

    public void ChangeEndSplineEvent(UnityAction _newEvent) {

        endOfSplineEvent.RemoveAllListeners();
        invokedEvent = false;
        endOfSplineEvent.AddListener(() => _newEvent());
    }

    public void ChangeSpeedParams(float _newTargetSpeed, float _newSpeedChange) {

        targetSpeed = _newTargetSpeed;
        speedChangingSpeed = _newSpeedChange;
        canChangeSpeed = true;
    }
}
