using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class CharacterHeadTrack : MonoBehaviour
    {
        private const float AIM_DISTANCE = 10f;
        private const float VISION_ANGLE = 220f;

        private const float MOVE_HEAD_SPEED = 0.15f;

        private const float MIN_WEIGHT = 0.0f;
        private const float MAX_WEIGHT = 0.8f;

        private class TrackInfo
        {
            public enum TrackState
            {
                TRACKING_POSITION,
                NOT_TRACKING
            }

            public TrackState currentTrackState;
            public Vector3 currentPosition;
            public float currentWeight;
            private float moveHeadSpeed = MOVE_HEAD_SPEED;

            private Vector3 _positionSpeed = Vector3.zero;
            private float _weightSpeed;

            public Transform headTransform;
            public Character character;
            public CharacterAnimator characterAnimator;

            public TrackInfo(Character character)
            {
                this.character = character;
                characterAnimator = this.character.GetCharacterAnimator();

                if (characterAnimator != null)
                {
                    headTransform = characterAnimator.GetHeadTransform();
                }

                currentTrackState = TrackState.NOT_TRACKING;
                currentPosition = Vector3.zero;
                currentWeight = 0.0f;

                _positionSpeed = Vector3.zero;
                _weightSpeed = 0f;
            }

            public void UpdateInfo(TrackState nextTrackState, Vector3 targetPosition = default)
            {
                float targetWeight;

                if (nextTrackState == TrackState.NOT_TRACKING)
                {
                    Vector3 direction = character.transform.TransformDirection(Vector3.forward);
                    if (headTransform != null && characterAnimator.useSmartHeadIK)
                    {
                        Vector3 aimDirection = character.characterLocomotion.GetAimDirection();
                        if (aimDirection != Vector3.zero &&
                            Vector3.Angle(aimDirection, direction) < VISION_ANGLE / 2f)
                        {
                            direction = aimDirection;
                        }
                    }

                    targetWeight = MAX_WEIGHT;
                    targetPosition = headTransform.position + direction * AIM_DISTANCE;
                }
                else
                {
                    targetWeight = MAX_WEIGHT;
                }

                currentPosition = Vector3.SmoothDamp(
                    currentPosition,
                    targetPosition,
                    ref _positionSpeed,
                    moveHeadSpeed
                );

                currentWeight = Mathf.SmoothDamp(
                    currentWeight,
                    targetWeight,
                    ref _weightSpeed,
                    moveHeadSpeed
                );

                currentTrackState = nextTrackState;
            }

            public void ChangeTrackTarget(float moveHeadSpeed = MOVE_HEAD_SPEED)
            {
                this.moveHeadSpeed = moveHeadSpeed;
                _weightSpeed = 0.0f;
                _positionSpeed = Vector3.zero;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        private TrackInfo trackInfo;

        private Target headTarget;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        // MAIN METHODS: --------------------------------------------------------------------------

        private void Awake()
        {
            animator = gameObject.GetComponentInChildren<Animator>();
            if (animator == null || !animator.isHuman) return;

            Character character = gameObject.GetComponentInParent<Character>();
            trackInfo = new TrackInfo(character);
            headTarget = new Target();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null || !animator.isHuman) return;
            eventBeforeIK.Invoke(layerIndex);

            if (headTarget == null || !headTarget.HasTarget())
            {
                trackInfo.UpdateInfo(TrackInfo.TrackState.NOT_TRACKING);
            }
            else
            {
                trackInfo.UpdateInfo(TrackInfo.TrackState.TRACKING_POSITION, headTarget.GetPosition());
            }

            animator.SetLookAtPosition(trackInfo.currentPosition);
            animator.SetLookAtWeight(trackInfo.currentWeight);

            eventAfterIK.Invoke(layerIndex);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Track(Vector3 position, float headSpeed = MOVE_HEAD_SPEED)
        {
            headTarget = new TargetPosition(position);
            trackInfo.ChangeTrackTarget(headSpeed);
        }

        public void Track(Transform transform, float headSpeed = MOVE_HEAD_SPEED)
        {
            headTarget = new TargetTransform(transform);
            trackInfo.ChangeTrackTarget(headSpeed);
        }

        public void TrackPlayer(float headSpeed = MOVE_HEAD_SPEED)
        {
            headTarget = new TargetPlayer();
            trackInfo.ChangeTrackTarget(headSpeed);
        }

        public void TrackCamera(float headSpeed = MOVE_HEAD_SPEED)
        {
            headTarget = new TargetCamera();
            trackInfo.ChangeTrackTarget(headSpeed);
        }

        public void Untrack(float headSpeed = MOVE_HEAD_SPEED)
        {
            headTarget = new Target();
            trackInfo.ChangeTrackTarget(headSpeed);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // TARGET CLASSES: ------------------------------------------------------------------------

        private class Target
        {
            public virtual Vector3 GetPosition()
            {
                return Vector3.zero;
            }

            public virtual bool HasTarget()
            {
                return false;
            }
        }

        private class TargetPosition : Target
        {
            public Vector3 position = Vector3.zero;

            public TargetPosition(Vector3 position)
            {
                this.position = position;
            }

            public override bool HasTarget()
            {
                return true;
            }

            public override Vector3 GetPosition()
            {
                return position;
            }
        }

        private class TargetTransform : Target
        {
            public Transform transform;

            public TargetTransform(Transform transform = null)
            {
                this.transform = transform;

                if (transform != null)
                {
                    CharacterAnimator charAnimator = transform.GetComponent<CharacterAnimator>();
                    if (charAnimator != null) this.transform = charAnimator.GetHeadTransform();
                }
            }

            public override bool HasTarget()
            {
                return transform != null;
            }

            public override Vector3 GetPosition()
            {
                if (transform == null) return Vector3.zero;
                return transform.position;
            }
        }

        private class TargetPlayer : TargetTransform
        {
            public TargetPlayer()
            {
                if (HookPlayer.Instance == null) return;

                Transform target = HookPlayer.Instance.transform;
                if (HookPlayer.Instance.Get<CharacterAnimator>() != null)
                {
                    target = HookPlayer.Instance.Get<CharacterAnimator>().GetHeadTransform();
                }

                transform = target;
            }
        }

        private class TargetCamera : TargetTransform
        {
            public TargetCamera()
            {
                if (HookCamera.Instance == null) return;
                transform = HookCamera.Instance.transform;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmosSelected()
        {
            if (trackInfo.characterAnimator != null && trackInfo.characterAnimator.useSmartHeadIK)
            {
                float radius = 0.3f;
                Vector3 position = trackInfo.headTransform.position;
                Vector3 direction = trackInfo.character.characterLocomotion.GetAimDirection();

                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f);
                Gizmos.DrawWireSphere(position, radius);

                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.75f);
                Gizmos.DrawCube(position + direction * radius, Vector3.one * 0.02f);
            }
        }
    }
}