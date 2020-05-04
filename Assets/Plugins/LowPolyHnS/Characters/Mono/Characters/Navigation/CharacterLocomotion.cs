using System;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    [Serializable]
    public class CharacterLocomotion
    {
        public enum ANIM_CONSTRAINT
        {
            NONE,
            KEEP_MOVEMENT,
            KEEP_POSITION
        }

        public enum LOCOMOTION_SYSTEM
        {
            CharacterController,
            NavigationMeshAgent
        }

        public enum FACE_DIRECTION
        {
            MovementDirection,
            CameraDirection,
            Target,
            GroundPlaneCursor,
            GroundPlaneCursorDelta
        }

        public enum OVERRIDE_FACE_DIRECTION
        {
            None = -1,
            MovementDirection = FACE_DIRECTION.MovementDirection,
            CameraDirection = FACE_DIRECTION.CameraDirection,
            Target = FACE_DIRECTION.Target,
            GroundPlaneCursor = FACE_DIRECTION.GroundPlaneCursor,
            GroundPlaneCursorDelta = FACE_DIRECTION.GroundPlaneCursorDelta
        }

        public enum STEP
        {
            Any,
            Left,
            Right
        }

        // CONSTANTS: -----------------------------------------------------------------------------

        private const float JUMP_COYOTE_TIME = 0.3f;
        private const float MAX_GROUND_VSPEED = -9.8f;
        private const float GROUND_TIME_OFFSET = 0.1f;

        private const float ACCELERATION = 25f;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool isControllable = true;
        public bool isBusy = false;

        public float runSpeed = 4.0f;
        [Range(0, 720f)] public float angularSpeed = 540f;
        public float gravity = -9.81f;
        public float maxFallSpeed = -100f;

        public bool canRun = true;
        public bool canJump = true;
        public float jumpForce = 6.0f;
        public int jumpTimes = 1;
        public float timeBetweenJumps = 0.5f;
        public float pushForce = 1.0f;

        [HideInInspector] public Vector3 terrainNormal = Vector3.up;
        [HideInInspector] public float verticalSpeed;

        // ADVANCED PROPERTIES: -------------------------------------------------------------------

        public OVERRIDE_FACE_DIRECTION overrideFaceDirection = OVERRIDE_FACE_DIRECTION.None;
        public TargetPosition overrideFaceDirectionTarget = new TargetPosition();

        public FACE_DIRECTION faceDirection = FACE_DIRECTION.MovementDirection;
        public TargetPosition faceDirectionTarget = new TargetPosition();

        [Tooltip("Check this if you want to use Unity's NavMesh and have a map baked")]
        public bool canUseNavigationMesh = false;

        // INNER PROPERTIES: ----------------------------------------------------------------------

        private float lastGroundTime;
        private float lastJumpTime;
        private int jumpChain;

        [HideInInspector] public Character character;

        [HideInInspector] public ANIM_CONSTRAINT animatorConstraint = ANIM_CONSTRAINT.NONE;
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public NavMeshAgent navmeshAgent;

        public LOCOMOTION_SYSTEM currentLocomotionType { get; private set; }
        public ILocomotionSystem currentLocomotionSystem { get; private set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            lastGroundTime = Time.time;
            lastJumpTime = Time.time;

            this.character = character;
            characterController = this.character.GetComponent<CharacterController>();

            currentLocomotionType = LOCOMOTION_SYSTEM.CharacterController;

            GenerateNavmeshAgent();
            SetDirectionalDirection(Vector3.zero);
        }

        // UPDATE: --------------------------------------------------------------------------------

        public void Update()
        {
            currentLocomotionType = LOCOMOTION_SYSTEM.CharacterController;
            if (currentLocomotionSystem != null)
            {
                currentLocomotionType = currentLocomotionSystem.Update();
            }

            switch (currentLocomotionType)
            {
                case LOCOMOTION_SYSTEM.CharacterController:
                    UpdateVerticalSpeed(characterController.isGrounded);
                    break;

                case LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    UpdateVerticalSpeed(!navmeshAgent.isOnOffMeshLink);
                    break;
            }

            UpdateCharacterState(currentLocomotionType);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Dash(Vector3 direction, float impulse, float duration, float drag)
        {
            SetDirectionalDirection(Vector3.zero);
            currentLocomotionSystem.Dash(direction, impulse, duration, drag);
        }

        public void RootMovement(float impulse, float duration, float gravityInfluence,
            AnimationCurve acForward, AnimationCurve acSides, AnimationCurve acVertical)
        {
            SetDirectionalDirection(Vector3.zero);
            currentLocomotionSystem.RootMovement(
                impulse, duration, gravityInfluence,
                acForward, acSides, acVertical
            );
        }

        public int Jump()
        {
            return Jump(jumpForce);
        }

        public int Jump(float jumpForce)
        {
            bool isGrounded = characterController.isGrounded ||
                              Time.time < lastGroundTime + JUMP_COYOTE_TIME;

            bool jumpDelay = lastJumpTime + timeBetweenJumps < Time.time;
            bool jumpNumber = isGrounded || jumpChain < jumpTimes;
            if (canJump && jumpNumber && jumpDelay)
            {
                verticalSpeed = jumpForce;
                lastJumpTime = Time.time;
                if (character.onJump != null)
                {
                    character.onJump.Invoke(jumpChain);
                }

                jumpChain++;

                return jumpChain;
            }

            return -1;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            Teleport(position);
            character.transform.rotation = rotation;

            Vector3 direction = rotation * Vector3.forward;

            SetDirectionalDirection(direction);
            currentLocomotionSystem.movementDirection = direction;
        }

        public void Teleport(Vector3 position)
        {
            switch (currentLocomotionType)
            {
                case LOCOMOTION_SYSTEM.CharacterController:
                    character.transform.position = position;
                    break;

                case LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    character.transform.position = position;
                    character.characterLocomotion.navmeshAgent.Warp(position);
                    break;
            }
        }

        public void SetAnimatorConstraint(ANIM_CONSTRAINT constraint)
        {
            animatorConstraint = constraint;
        }

        public void ChangeHeight(float height)
        {
            if (characterController != null)
            {
                characterController.height = height;
                characterController.center = Vector3.up * (height / 2.0f);
            }

            if (navmeshAgent != null)
            {
                navmeshAgent.height = height;
            }
        }

        public void SetIsControllable(bool isControllable)
        {
            if (isControllable == this.isControllable) return;
            this.isControllable = isControllable;

            if (!isControllable) SetDirectionalDirection(Vector3.zero);
            if (character.onIsControllable != null)
            {
                character.onIsControllable.Invoke(this.isControllable);
            }
        }

        public Vector3 GetAimDirection()
        {
            return currentLocomotionSystem.aimDirection;
        }

        public Vector3 GetMovementDirection()
        {
            return currentLocomotionSystem.movementDirection;
        }

        // PUBLIC LOCOMOTION METHODS: -------------------------------------------------------------

        public void SetDirectionalDirection(Vector3 direction, ILocomotionSystem.TargetRotation rotation = null)
        {
            ChangeLocomotionSystem<LocomotionSystemDirectional>();
            ((LocomotionSystemDirectional) currentLocomotionSystem).SetDirection(direction, rotation);
        }

        public void SetTankDirection(Vector3 direction, float rotationY)
        {
            ChangeLocomotionSystem<LocomotionSystemTank>();
            ((LocomotionSystemTank) currentLocomotionSystem).SetDirection(
                direction,
                rotationY
            );
        }

        public void SetTarget(Ray ray, LayerMask layerMask, ILocomotionSystem.TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget) currentLocomotionSystem)
                .SetTarget(ray, layerMask, rotation, stopThreshold, callback);
        }

        public void SetTarget(Vector3 position, ILocomotionSystem.TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget) currentLocomotionSystem)
                .SetTarget(position, rotation, stopThreshold, callback);
        }

        public void FollowTarget(Transform target, float minRadius, float maxRadius)
        {
            ChangeLocomotionSystem<LocomotionSystemFollow>();
            ((LocomotionSystemFollow) currentLocomotionSystem).SetFollow(target, minRadius, maxRadius);
        }

        public void Stop(ILocomotionSystem.TargetRotation rotation = null, UnityAction callback = null)
        {
            ChangeLocomotionSystem<LocomotionSystemTarget>();
            ((LocomotionSystemTarget) currentLocomotionSystem).Stop(rotation, callback);
        }

        public void SetRotation(Vector3 direction)
        {
            ChangeLocomotionSystem<LocomotionSystemRotation>();
            ((LocomotionSystemRotation) currentLocomotionSystem).SetDirection(direction);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void GenerateNavmeshAgent()
        {
            if (!canUseNavigationMesh) return;

            if (navmeshAgent == null) navmeshAgent = character.gameObject.GetComponent<NavMeshAgent>();
            if (navmeshAgent == null) navmeshAgent = character.gameObject.AddComponent<NavMeshAgent>();

            navmeshAgent.updatePosition = false;
            navmeshAgent.updateRotation = false;
            navmeshAgent.updateUpAxis = false;
            navmeshAgent.radius = characterController.radius;
            navmeshAgent.height = characterController.height;
            navmeshAgent.acceleration = ACCELERATION;
        }

        private void ChangeLocomotionSystem<TLS>() where TLS : ILocomotionSystem, new()
        {
            if (currentLocomotionSystem != null && typeof(TLS) == currentLocomotionSystem.GetType()) return;
            if (currentLocomotionSystem != null) currentLocomotionSystem.OnDestroy();

            currentLocomotionSystem = new TLS();
            currentLocomotionSystem.Setup(this);
        }

        private void UpdateVerticalSpeed(bool isGrounded)
        {
            verticalSpeed += gravity * Time.deltaTime;
            if (isGrounded)
            {
                if (Time.time - lastGroundTime > JUMP_COYOTE_TIME + Time.deltaTime &&
                    character.onLand != null)
                {
                    character.onLand.Invoke(verticalSpeed);
                }

                jumpChain = 0;
                lastGroundTime = Time.time;
                verticalSpeed = Mathf.Max(verticalSpeed, MAX_GROUND_VSPEED);
            }

            verticalSpeed = Mathf.Max(verticalSpeed, maxFallSpeed);
        }

        private void UpdateCharacterState(LOCOMOTION_SYSTEM locomotionSystem)
        {
            Vector3 worldVelocity = Vector3.zero;
            bool isSliding = currentLocomotionSystem.isSliding;
            bool isGrounded = true;

            switch (locomotionSystem)
            {
                case LOCOMOTION_SYSTEM.CharacterController:
                    worldVelocity = characterController.velocity;
                    isGrounded = characterController.isGrounded ||
                                 Time.time - lastGroundTime < GROUND_TIME_OFFSET;
                    break;

                case LOCOMOTION_SYSTEM.NavigationMeshAgent:
                    worldVelocity = navmeshAgent.velocity == Vector3.zero
                        ? characterController.velocity
                        : navmeshAgent.velocity;
                    isGrounded = !navmeshAgent.isOnOffMeshLink ||
                                 Time.time - lastGroundTime < GROUND_TIME_OFFSET;
                    break;
            }

            Vector3 localVelocity = character.transform.InverseTransformDirection(worldVelocity);
            character.characterState.forwardSpeed = localVelocity;
            character.characterState.sidesSpeed = Mathf.Atan2(localVelocity.x, localVelocity.z);
            character.characterState.verticalSpeed = worldVelocity.y;

            character.characterState.pivotSpeed = currentLocomotionSystem.pivotSpeed;

            character.characterState.isGrounded = isGrounded ? 1f : 0f;
            character.characterState.isSliding = isSliding ? 1f : 0f;
            character.characterState.isDashing = currentLocomotionSystem.isDashing ? 1f : 0f;
            character.characterState.normal = terrainNormal;
        }
    }
}