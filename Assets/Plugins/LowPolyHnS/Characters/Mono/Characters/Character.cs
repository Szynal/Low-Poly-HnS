using System;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("LowPolyHnS/Characters/Character", 100)]
    public class Character : GlobalID, IGameSave
    {
        [Serializable]
        public class State
        {
            public Vector3 forwardSpeed;
            public float sidesSpeed;
            public float pivotSpeed;
            public bool targetLock;
            public float isGrounded;
            public float isSliding;
            public float isDashing;
            public float verticalSpeed;
            public Vector3 normal;

            public State()
            {
                forwardSpeed = Vector3.zero;
                sidesSpeed = 0f;
                targetLock = false;
                isGrounded = 1.0f;
                isSliding = 0.0f;
                isDashing = 0.0f;
                verticalSpeed = 0f;
                normal = Vector3.zero;
            }
        }

        [Serializable]
        public class SaveData
        {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;
        }

        [Serializable]
        public class OnLoadSceneData
        {
            public bool active { get; private set; }
            public Vector3 position { get; private set; }
            public Quaternion rotation { get; private set; }

            public OnLoadSceneData(Vector3 position, Quaternion rotation)
            {
                active = true;
                this.position = position;
                this.rotation = rotation;
            }

            public void Consume()
            {
                active = false;
            }
        }

        public class LandEvent : UnityEvent<float>
        {
        }

        public class JumpEvent : UnityEvent<int>
        {
        }

        public class DashEvent : UnityEvent
        {
        }

        public class StepEvent : UnityEvent<CharacterLocomotion.STEP>
        {
        }

        public class IsControllableEvent : UnityEvent<bool>
        {
        }

        protected const string ERR_NOCAM = "No Main Camera found.";

        // PROPERTIES: ----------------------------------------------------------------------------

        public CharacterLocomotion characterLocomotion;

        public State characterState = new State();
        private CharacterAnimator animator;
        private CharacterRagdoll ragdoll;

        public JumpEvent onJump = new JumpEvent();
        public LandEvent onLand = new LandEvent();
        public DashEvent onDash = new DashEvent();
        public StepEvent onStep = new StepEvent();

        public IsControllableEvent onIsControllable = new IsControllableEvent();

        public bool save;
        protected SaveData initSaveData = new SaveData();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            CharacterAwake();

            initSaveData = new SaveData
            {
                position = transform.position,
                rotation = transform.rotation
            };

            if (save)
            {
                SaveLoadManager.Instance.Initialize(this);
            }
        }

        protected void CharacterAwake()
        {
            if (!Application.isPlaying) return;
            animator = GetComponent<CharacterAnimator>();
            characterLocomotion.Setup(this);

            if (animator != null && animator.autoInitializeRagdoll)
            {
                InitializeRagdoll();
            }
        }

        protected void OnDestroy()
        {
            OnDestroyGID();
            if (!Application.isPlaying) return;

            if (save && !exitingApplication)
            {
                SaveLoadManager.Instance.OnDestroyIGameSave(this);
            }
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            if (!Application.isPlaying) return;
            CharacterUpdate();
        }

        protected void CharacterUpdate()
        {
            if (ragdoll != null && ragdoll.GetState() != CharacterRagdoll.State.Normal) return;

            characterLocomotion.Update();
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying) return;
            if (ragdoll != null && ragdoll.GetState() != CharacterRagdoll.State.Normal)
            {
                ragdoll.Update();
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public State GetCharacterState()
        {
            return characterState;
        }

        public void SetRagdoll(bool active, bool autoStand = false)
        {
            if (active && ragdoll.GetState() != CharacterRagdoll.State.Normal) return;
            if (!active && ragdoll.GetState() == CharacterRagdoll.State.Normal) return;

            characterLocomotion.characterController.enabled = !active;
            animator.animator.enabled = !active;

            Transform model = animator.animator.transform;
            switch (active)
            {
                case true:
                    ragdoll.Ragdoll(true, autoStand);
                    model.SetParent(null, true);
                    break;

                case false:
                    model.SetParent(transform, true);
                    ragdoll.Ragdoll(false, autoStand);
                    break;
            }
        }

        public void InitializeRagdoll()
        {
            ragdoll = new CharacterRagdoll(this);
        }

        // GETTERS: -------------------------------------------------------------------------------

        public bool IsControllable()
        {
            if (characterLocomotion == null) return false;
            return characterLocomotion.isControllable;
        }

        public bool IsRagdoll()
        {
            return ragdoll != null && ragdoll.GetState() != CharacterRagdoll.State.Normal;
        }

        public int GetCharacterMotion()
        {
            if (characterState == null) return 0;
            if (characterLocomotion == null) return 0;

            float speed = Mathf.Abs(characterState.forwardSpeed.magnitude);
            if (Mathf.Approximately(speed, 0.0f)) return 0;
            if (characterLocomotion.canRun && speed > characterLocomotion.runSpeed / 2.0f)
            {
                return 2;
            }

            return 1;
        }

        public bool IsGrounded()
        {
            if (characterState == null) return true;
            return Mathf.Approximately(characterState.isGrounded, 1.0f);
        }

        public CharacterAnimator GetCharacterAnimator()
        {
            return animator;
        }

        // JUMP: ----------------------------------------------------------------------------------

        public bool Dash(Vector3 direction, float impulse, float duration, float drag = 10f)
        {
            if (characterLocomotion.isBusy) return false;

            characterLocomotion.Dash(direction, impulse, duration, drag);
            if (animator != null) animator.Dash();
            if (onDash != null) onDash.Invoke();
            return true;
        }

        public void RootMovement(float impulse, float duration, float gravityInfluence,
            AnimationCurve acForward, AnimationCurve acSides, AnimationCurve acVertical)
        {
            characterLocomotion.RootMovement(
                impulse, duration, gravityInfluence,
                acForward, acSides, acVertical
            );
        }

        public void Jump(float force)
        {
            int jumpChain = characterLocomotion.Jump(force);
            if (jumpChain >= 0 && animator != null)
            {
                animator.Jump();
            }
        }

        public void Jump()
        {
            int jumpChain = characterLocomotion.Jump();
            if (jumpChain >= 0 && animator != null)
            {
                animator.Jump(jumpChain);
            }
        }

        // HEAD TRACKER: --------------------------------------------------------------------------

        public CharacterHeadTrack GetHeadTracker()
        {
            if (animator == null) return null;
            return animator.GetHeadTracker();
        }

        // FLOOR COLLISION: -----------------------------------------------------------------------

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!Application.isPlaying) return;

            float coefficient = characterLocomotion.pushForce;
            if (coefficient < float.Epsilon) return;

            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 90f) characterLocomotion.terrainNormal = hit.normal;

            Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
            if (angle <= 90f && angle >= 5f && hitRigidbody != null && !hitRigidbody.isKinematic)
            {
                Vector3 force = hit.controller.velocity * coefficient / Time.fixedDeltaTime;
                hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Force);
            }
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            if (ragdoll != null) ragdoll.OnDrawGizmos();
        }

        // GAME SAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            string uniqueName = string.Format(
                "character:{0}",
                GetUniqueCharacterID()
            );

            return uniqueName;
        }

        protected virtual string GetUniqueCharacterID()
        {
            return GetID();
        }

        public Type GetSaveDataType()
        {
            return typeof(SaveData);
        }

        public object GetSaveData()
        {
            return new SaveData
            {
                position = transform.position,
                rotation = transform.rotation
            };
        }

        public void ResetData()
        {
            transform.position = initSaveData.position;
            transform.rotation = initSaveData.rotation;
        }

        public void OnLoad(object generic)
        {
            SaveData container = generic as SaveData;
            if (container == null) return;

            transform.position = container.position;
            transform.rotation = container.rotation;
        }
    }
}