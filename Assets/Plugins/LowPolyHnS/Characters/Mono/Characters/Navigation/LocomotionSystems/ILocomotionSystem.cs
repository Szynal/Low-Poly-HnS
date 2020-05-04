using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    public abstract class ILocomotionSystem
    {
        public class TargetRotation
        {
            public bool hasRotation;
            public Quaternion rotation;

            public TargetRotation(bool hasRotation = false, Vector3 direction = default)
            {
                this.hasRotation = hasRotation;
                rotation = hasRotation && direction != Vector3.zero
                    ? Quaternion.LookRotation(direction)
                    : Quaternion.identity;
            }
        }

        protected class DirectionData
        {
            public CharacterLocomotion.FACE_DIRECTION direction;
            public TargetPosition target;

            public DirectionData(CharacterLocomotion.FACE_DIRECTION direction, TargetPosition target)
            {
                this.direction = direction;
                this.target = target;
            }
        }

        // CONSTANT PROPERTIES: -------------------------------------------------------------------

        protected static readonly Vector3 HORIZONTAL_PLANE = new Vector3(1, 0, 1);

        private const string AXIS_MOUSE_X = "Mouse X";
        private const string AXIS_MOUSE_Y = "Mouse Y";

        protected const float SLOW_THRESHOLD = 1.0f;
        protected const float STOP_THRESHOLD = 0.05f;
        protected const float SLIDING_SMOOTH = 0.35f;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected CharacterLocomotion characterLocomotion;
        public Vector3 aimDirection;
        public Vector3 movementDirection;
        public float pivotSpeed;

        public bool isSliding;
        protected Vector3 slideDirection = Vector3.zero;

        public bool isDashing { private set; get; }
        protected Vector3 dashVelocity = Vector3.zero;

        private float dashStartTime = -100f;
        private float dashDuration;
        private float dashDrag = 10f;

        public bool isRootMoving { private set; get; }
        protected Vector3 rootMoveVelocity = Vector3.zero;

        private AnimationCurve rootMoveCurveForward = new AnimationCurve();
        private AnimationCurve rootMoveCurveSides = new AnimationCurve();
        private AnimationCurve rootMoveCurveVertical = new AnimationCurve();

        private float rootMoveDeltaForward;
        private float rootMoveDeltaSides;
        private float rootMoveDeltaVertical;

        private float rootMoveStartTime = -100f;
        private float rootMoveImpulse;
        private float rootMoveGravity;
        private float rootMoveDuration;

        private float slidingValue;
        private float slidingSpeed;

        private DirectionData cacheDirectionData = new DirectionData(
            CharacterLocomotion.FACE_DIRECTION.MovementDirection,
            new TargetPosition(TargetPosition.Target.Player)
        );

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Setup(CharacterLocomotion characterLocomotion)
        {
            this.characterLocomotion = characterLocomotion;
        }

        public void Dash(Vector3 direction, float impulse, float duration, float drag)
        {
            isDashing = true;
            dashStartTime = Time.time;
            dashDuration = duration;
            dashDrag = drag;

            dashVelocity = direction.normalized * (
                               impulse * Mathf.Log(1f / (Time.deltaTime * dashDrag + 1)) / -Time.deltaTime
                           );
        }

        public void RootMovement(float impulse, float duration, float gravityInfluence,
            AnimationCurve acForward, AnimationCurve acSides, AnimationCurve acVertical)
        {
            isRootMoving = true;
            rootMoveImpulse = impulse;
            rootMoveStartTime = Time.time;
            rootMoveDuration = duration;
            rootMoveGravity = gravityInfluence;

            rootMoveCurveForward = acForward;
            rootMoveCurveSides = acSides;
            rootMoveCurveVertical = acVertical;

            rootMoveDeltaForward = 0f;
            rootMoveDeltaSides = 0f;
            rootMoveDeltaVertical = 0f;
        }

        public void StopRootMovement()
        {
            isRootMoving = false;
        }

        // ABSTRACT & VIRTUAL METHODS: ------------------------------------------------------------

        public virtual CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            if (isRootMoving)
            {
                // TODO: Maybe add some drag?
                if (Time.time >= rootMoveStartTime + rootMoveDuration)
                {
                    isRootMoving = false;
                }
            }

            if (isDashing)
            {
                if (Time.time >= dashStartTime + dashDuration)
                {
                    dashVelocity /= 1 + dashDrag * Time.deltaTime;
                }

                if (dashVelocity.magnitude < characterLocomotion.runSpeed)
                {
                    isDashing = false;
                }
            }

            return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
        }

        public abstract void OnDestroy();

        // CHARACTER CONTROLLER METHODS: ----------------------------------------------------------

        protected Quaternion UpdateRotation(Vector3 targetDirection)
        {
            Quaternion targetRotation = characterLocomotion.character.transform.rotation;
            aimDirection = characterLocomotion.character.transform.TransformDirection(Vector3.forward);
            movementDirection = targetDirection == Vector3.zero
                ? aimDirection
                : targetDirection.normalized;

            DirectionData faceDirection = GetFaceDirection();

            if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
            {
                Quaternion srcRotation = characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                aimDirection = dstRotation * Vector3.forward;

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.CameraDirection &&
                     HookCamera.Instance != null)
            {
                Vector3 camDirection = HookCamera.Instance.transform.TransformDirection(Vector3.forward);
                aimDirection = camDirection;

                camDirection.Scale(HORIZONTAL_PLANE);

                Quaternion srcRotation = characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(camDirection);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.Target)
            {
                Vector3 target = faceDirection.target.GetPosition(characterLocomotion.character.gameObject);
                Vector3 direction = target - characterLocomotion.character.transform.position;
                aimDirection = direction;

                direction.Scale(HORIZONTAL_PLANE);

                Quaternion srcRotation = characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(direction);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.GroundPlaneCursor)
            {
                Camera camera = null;
                if (camera == null)
                {
                    if (HookCamera.Instance != null) camera = HookCamera.Instance.Get<Camera>();
                    if (camera == null && Camera.main != null) camera = Camera.main;
                }

                Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
                Transform character = characterLocomotion.character.transform;

                Plane plane = new Plane(Vector3.up, character.position);
                float rayDistance = 0.0f;

                if (plane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    Vector3 target = Vector3.MoveTowards(character.position, cursor, 1f);
                    Vector3 direction = target - characterLocomotion.character.transform.position;
                    direction.Scale(HORIZONTAL_PLANE);

                    Quaternion srcRotation = character.rotation;
                    Quaternion dstRotation = Quaternion.LookRotation(direction);
                    aimDirection = dstRotation * Vector3.forward;

                    targetRotation = Quaternion.RotateTowards(
                        srcRotation,
                        dstRotation,
                        Time.deltaTime * characterLocomotion.angularSpeed
                    );
                }
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.GroundPlaneCursorDelta)
            {
                Camera camera = null;
                if (camera == null)
                {
                    if (HookCamera.Instance != null) camera = HookCamera.Instance.Get<Camera>();
                    if (camera == null && Camera.main != null) camera = Camera.main;
                }

                Vector3 deltaDirection = new Vector3(
                    Input.GetAxisRaw(AXIS_MOUSE_X),
                    0f,
                    Input.GetAxisRaw(AXIS_MOUSE_Y)
                );

                if (Mathf.Abs(deltaDirection.sqrMagnitude) > 0.05f)
                {
                    deltaDirection = camera.transform.TransformDirection(deltaDirection);
                    deltaDirection.Scale(HORIZONTAL_PLANE);
                    deltaDirection.Normalize();

                    Quaternion srcRotation = characterLocomotion.character.transform.rotation;
                    Quaternion dstRotation = Quaternion.LookRotation(deltaDirection);
                    aimDirection = dstRotation * Vector3.forward;

                    targetRotation = Quaternion.RotateTowards(
                        srcRotation,
                        dstRotation,
                        Time.deltaTime * characterLocomotion.angularSpeed
                    );
                }
            }

            return targetRotation;
        }

        protected float CalculateSpeed(Vector3 targetDirection, bool isGrounded)
        {
            float speed = characterLocomotion.canRun
                ? characterLocomotion.runSpeed
                : characterLocomotion.runSpeed / 2.0f;

            DirectionData direction = GetFaceDirection();

            if (direction.direction == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
            {
                Quaternion srcRotation = characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                float angle = Quaternion.Angle(srcRotation, dstRotation) / 180.0f;
                float speedDampening = Mathf.Clamp(1.0f - angle, 0.5f, 1.0f);
                speed *= speedDampening;
            }

            return speed;
        }

        protected virtual void UpdateAnimationConstraints(ref Vector3 targetDirection, ref Quaternion targetRotation)
        {
            if (characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT)
            {
                if (targetDirection == Vector3.zero)
                {
                    Transform characterTransform = characterLocomotion.characterController.transform;
                    targetDirection = characterTransform.TransformDirection(Vector3.forward);
                }
            }

            if (characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION)
            {
                targetDirection = Vector3.zero;
                targetRotation = characterLocomotion.characterController.transform.rotation;
            }
        }

        protected virtual void UpdateSliding()
        {
            float slopeAngle = Vector3.Angle(Vector3.up, characterLocomotion.terrainNormal);
            bool frameSliding = characterLocomotion.character.IsGrounded() &&
                                slopeAngle > characterLocomotion.characterController.slopeLimit;

            slidingValue = Mathf.SmoothDamp(
                slidingValue,
                frameSliding ? 1f : 0f,
                ref slidingSpeed,
                SLIDING_SMOOTH
            );

            isSliding = slidingValue > 0.5f;
            if (isSliding)
            {
                isSliding = true;
                slideDirection = Vector3.Reflect(
                                     Vector3.down, characterLocomotion.terrainNormal
                                 ) * characterLocomotion.runSpeed;
            }
            else
            {
                slideDirection = Vector3.zero;
            }
        }

        protected void UpdateRootMovement(Vector3 verticalMovement)
        {
            float t = (Time.time - rootMoveStartTime) / rootMoveDuration;
            float deltaForward = rootMoveCurveForward.Evaluate(t) * rootMoveImpulse;
            float deltaSides = rootMoveCurveSides.Evaluate(t) * rootMoveImpulse;
            float deltaVertical = rootMoveCurveVertical.Evaluate(t) * rootMoveImpulse;

            Vector3 movement = new Vector3(
                deltaSides - rootMoveDeltaSides,
                deltaVertical - rootMoveDeltaVertical,
                deltaForward - rootMoveDeltaForward
            );

            movement += verticalMovement * rootMoveGravity * Time.deltaTime;

            characterLocomotion.characterController.Move(
                characterLocomotion.character.transform.TransformDirection(movement)
            );

            rootMoveDeltaForward = deltaForward;
            rootMoveDeltaSides = deltaSides;
            rootMoveDeltaVertical = deltaVertical;
        }

        protected DirectionData GetFaceDirection()
        {
            CharacterLocomotion.FACE_DIRECTION direction = characterLocomotion.faceDirection;
            TargetPosition target = characterLocomotion.faceDirectionTarget;

            if (characterLocomotion.overrideFaceDirection != CharacterLocomotion.OVERRIDE_FACE_DIRECTION.None)
            {
                direction = (CharacterLocomotion.FACE_DIRECTION) characterLocomotion.overrideFaceDirection;
                target = characterLocomotion.overrideFaceDirectionTarget;
            }

            cacheDirectionData.direction = direction;
            cacheDirectionData.target = target;

            return cacheDirectionData;
        }
    }
}