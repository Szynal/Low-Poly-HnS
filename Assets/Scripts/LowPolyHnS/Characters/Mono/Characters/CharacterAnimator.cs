using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("LowPolyHnS/Characters/Character Animator", 100)]
    public class CharacterAnimator : MonoBehaviour
    {
        private const float NORMAL_SMOOTH = 0.1f;
        private const float MAX_LAND_FORCE_SPEED = -10.0f;
        private const float FLOAT_ERROR_MARGIN = 0.01f;

        public const string LAYER_BASE = "Base";

        private const string EXC_NO_CHARACTER = "No CharacterNavigatorController found on gameObject";
        private const string EXC_NO_ANIMATOR = "No Animator attached to CharacterNavigationAnimator";

        private class AnimFloat
        {
            private bool setup;
            private float value;
            private float velocity;

            public float Get(float target, float smooth)
            {
                if (!setup)
                {
                    value = target;
                    velocity = 0.0f;
                    setup = true;
                }

                value = Mathf.SmoothDamp(
                    value,
                    target,
                    ref velocity,
                    smooth
                );

                if (value < FLOAT_ERROR_MARGIN && value > -FLOAT_ERROR_MARGIN)
                {
                    value = 0f;
                }

                return value;
            }

            public void Set(float value)
            {
                this.value = value;
                velocity = 0.0f;
            }
        }

        public class EventIK : UnityEvent<int>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Animator animator;
        private Character character;

        [SerializeField] protected CharacterState defaultState;

        private CharacterAnimatorEvents animEvents;
        private CharacterAnimation characterAnimation;
        private CharacterAttachments characterAttachments;
        private CharacterAnimatorRotation characterRotation;

        private CharacterHeadTrack headTrack;
        private CharacterFootIK footIK;
        private CharacterHandIK handIK;

        private bool stiffBody;

        private static readonly int HASH_MOVE_FORWARD_SPEED = Animator.StringToHash("MoveForward");
        private static readonly int HASH_MOVE_SIDES_SPEED = Animator.StringToHash("MoveSides");
        private static readonly int HASH_MOVE_TURN_SPEED = Animator.StringToHash("TurnSpeed");
        private static readonly int HASH_MOVEMENT_SPEED = Animator.StringToHash("Movement");
        private static readonly int HASH_IS_GROUNDED = Animator.StringToHash("IsGrounded");
        private static readonly int HASH_IS_SLIDING = Animator.StringToHash("IsSliding");
        private static readonly int HASH_IS_DASHING = Animator.StringToHash("IsDashing");
        private static readonly int HASH_VERTICAL_SPEED = Animator.StringToHash("VerticalSpeed");
        private static readonly int HASH_NORMAL_X = Animator.StringToHash("NormalX");
        private static readonly int HASH_NORMAL_Y = Animator.StringToHash("NormalY");
        private static readonly int HASH_NORMAL_Z = Animator.StringToHash("NormalZ");
        private static readonly int HASH_JUMP = Animator.StringToHash("Jump");
        private static readonly int HASH_JUMP_CHAIN = Animator.StringToHash("JumpChain");
        private static readonly int HASH_LAND = Animator.StringToHash("Land");
        private static readonly int HASH_LAND_FORCE = Animator.StringToHash("LandForce");
        private static readonly int HASH_TIME_SCALE = Animator.StringToHash("TimeScale");

        private Dictionary<int, AnimFloat> paramValues = new Dictionary<int, AnimFloat>();
        private float rotationVelocity;

        public bool useFootIK = true;
        public LayerMask footLayerMask = Physics.DefaultRaycastLayers;
        public bool useHandIK = true;
        public bool useSmartHeadIK = true;
        public bool useProceduralLanding = true;

        public bool autoInitializeRagdoll = true;

        [Tooltip("Total amount of mass of the character")]
        public float ragdollMass = 80f;

        [Tooltip("Time needed to confirm the ragdoll is stable before getting up"), Range(0.1f, 5.0f)]
        public float stableTimeout = 0.5f;

        public AnimationClip standFaceDown;
        public AnimationClip standFaceUp;

        public Action overrideLateUpdate;

        [Range(0f, 1f)] public float timeScaleCoefficient = 1f;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            if (animator != null) animator.applyRootMotion = false;

            character = gameObject.GetComponent<Character>();
            characterAnimation = new CharacterAnimation(this, defaultState);
            characterRotation = new CharacterAnimatorRotation();

            paramValues.Add(HASH_MOVE_FORWARD_SPEED, new AnimFloat());
            paramValues.Add(HASH_MOVE_SIDES_SPEED, new AnimFloat());
            paramValues.Add(HASH_MOVE_TURN_SPEED, new AnimFloat());
            paramValues.Add(HASH_MOVEMENT_SPEED, new AnimFloat());
            paramValues.Add(HASH_VERTICAL_SPEED, new AnimFloat());
            paramValues.Add(HASH_NORMAL_X, new AnimFloat());
            paramValues.Add(HASH_NORMAL_Y, new AnimFloat());
            paramValues.Add(HASH_NORMAL_Z, new AnimFloat());
            paramValues.Add(HASH_LAND_FORCE, new AnimFloat());
            paramValues.Add(HASH_IS_GROUNDED, new AnimFloat());
            paramValues.Add(HASH_IS_SLIDING, new AnimFloat());
            paramValues.Add(HASH_IS_DASHING, new AnimFloat());
        }

        private void Start()
        {
            character.onLand.AddListener(OnLand);
        }

        private void OnDestroy()
        {
            if (characterAnimation != null) characterAnimation.OnDestroy();
            if (animator != null) Destroy(animator.gameObject);
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            if (!animator.gameObject.activeInHierarchy) return;

            if (character == null) throw new UnityException(EXC_NO_CHARACTER);
            if (animator == null) throw new UnityException(EXC_NO_ANIMATOR);
            if (animEvents == null) GenerateAnimatorEvents();

            if (characterAttachments == null) GenerateCharacterAttachments();
            if (characterAnimation != null) characterAnimation.Update();

            if (useFootIK && footIK == null) GenerateFootIK();
            if (useHandIK && handIK == null) GenerateHandIK();
            if (useSmartHeadIK && headTrack == null)
            {
                if (GetHeadTracker() != null) headTrack.Untrack();
            }

            Quaternion rotation = characterRotation.Update();

            Character.State state = character.GetCharacterState();
            Vector3 direction = !character.enabled || state.forwardSpeed.magnitude < 0.01f
                ? Vector3.zero
                : state.forwardSpeed;

            direction = Quaternion.Euler(0f, -rotation.eulerAngles.y, 0f) * direction;

            switch (character.IsRagdoll())
            {
                case true:
                    rotation = animator.transform.localRotation;
                    break;

                case false:
                    rotation.eulerAngles = new Vector3(
                        rotation.eulerAngles.x,
                        Mathf.SmoothDampAngle(
                            animator.transform.localRotation.eulerAngles.y,
                            character.IsRagdoll()
                                ? animator.transform.localRotation.eulerAngles.y
                                : rotation.eulerAngles.y,
                            ref rotationVelocity, 1f
                        ),
                        rotation.eulerAngles.z
                    );
                    break;
            }

            animator.transform.localRotation = rotation;
            direction = Vector3.Scale(direction, Vector3.one * (1.0f / character.characterLocomotion.runSpeed));

            float paramMoveForwardSpeed = paramValues[HASH_MOVE_FORWARD_SPEED].Get(direction.z, 0.1f);
            float paramMoveSidesSpeed = paramValues[HASH_MOVE_SIDES_SPEED].Get(direction.x, 0.2f);
            float paramMovementSpeed = paramValues[HASH_MOVEMENT_SPEED].Get(
                Vector3.Scale(direction, new Vector3(1, 0, 1)).magnitude,
                0.1f
            );

            float paramMoveTurnSpeed = paramValues[HASH_MOVE_TURN_SPEED].Get(state.pivotSpeed, 0.1f);
            float paramVerticalSpeed = paramValues[HASH_VERTICAL_SPEED].Get(state.verticalSpeed, 0.2f);
            float paramIsGrounded = paramValues[HASH_IS_GROUNDED].Get(state.isGrounded, 0.1f);
            float paramIsSliding = paramValues[HASH_IS_SLIDING].Get(state.isSliding, 0.1f);
            float paramIsDashing = paramValues[HASH_IS_DASHING].Get(state.isDashing, 0.05f);
            float paramLandForce = paramValues[HASH_LAND_FORCE].Get(0f, 2f);

            animator.SetFloat(HASH_MOVE_FORWARD_SPEED, paramMoveForwardSpeed);
            animator.SetFloat(HASH_MOVE_SIDES_SPEED, paramMoveSidesSpeed);
            animator.SetFloat(HASH_MOVE_TURN_SPEED, paramMoveTurnSpeed);
            animator.SetFloat(HASH_MOVEMENT_SPEED, paramMovementSpeed);
            animator.SetFloat(HASH_IS_GROUNDED, paramIsGrounded);
            animator.SetFloat(HASH_IS_SLIDING, paramIsSliding);
            animator.SetFloat(HASH_IS_DASHING, paramIsDashing);
            animator.SetFloat(HASH_VERTICAL_SPEED, paramVerticalSpeed);
            animator.SetFloat(HASH_TIME_SCALE, Time.timeScale * timeScaleCoefficient);
            animator.SetFloat(HASH_LAND_FORCE, paramLandForce);

            Normals(state);
        }

        private void Normals(Character.State state)
        {
            Vector3 normal = Vector3.up;
            if (Mathf.Approximately(state.isGrounded, 1.0f))
            {
                normal = character.transform.InverseTransformDirection(state.normal);
            }

            float paramNormalX = paramValues[HASH_NORMAL_X].Get(normal.x, NORMAL_SMOOTH);
            float paramNormalY = paramValues[HASH_NORMAL_Y].Get(normal.y, NORMAL_SMOOTH);
            float paramNormalZ = paramValues[HASH_NORMAL_Z].Get(normal.z, NORMAL_SMOOTH);

            animator.SetFloat(HASH_NORMAL_X, paramNormalX);
            animator.SetFloat(HASH_NORMAL_Y, paramNormalY);
            animator.SetFloat(HASH_NORMAL_Z, paramNormalZ);
        }

        private void LateUpdate()
        {
            if (overrideLateUpdate != null)
            {
                overrideLateUpdate.Invoke();
                return;
            }

            if (stiffBody)
            {
                Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
                Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);

                spine.localRotation = spine.localRotation *
                                      Quaternion.Inverse(hips.localRotation);

                animator.GetBoneTransform(HumanBodyBones.Chest).localRotation = Quaternion.identity;
                animator.GetBoneTransform(HumanBodyBones.UpperChest).localRotation = Quaternion.identity;
                animator.GetBoneTransform(HumanBodyBones.Neck).localRotation = Quaternion.identity;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public CharacterHeadTrack GetHeadTracker()
        {
            if (headTrack == null)
            {
                headTrack = gameObject.GetComponentInChildren<CharacterHeadTrack>();
                if (headTrack == null && animator != null && animator.isHuman)
                {
                    headTrack = animator.gameObject.AddComponent<CharacterHeadTrack>();
                }
            }

            return headTrack;
        }

        public void Jump(int jumpChain = 0)
        {
            animator.SetInteger(HASH_JUMP_CHAIN, jumpChain);
            animator.SetTrigger(HASH_JUMP);
        }

        public void Dash()
        {
            paramValues[HASH_IS_DASHING].Set(1f);
        }

        public Transform GetHeadTransform()
        {
            if (!animator.isHuman) return transform;
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            return head ?? transform;
        }

        public void PlayGesture(AnimationClip clip, float speed, AvatarMask avatarMask = null,
            float transitionIn = 0.15f, float transitionOut = 0.15f)
        {
            characterAnimation.PlayGesture(clip, avatarMask, transitionIn, transitionOut, speed);
        }

        public void CrossFadeGesture(AnimationClip clip, float speed, AvatarMask avatarMask = null,
            float transitionIn = 0.15f, float transitionOut = 0.15f)
        {
            characterAnimation.CrossFadeGesture(clip, avatarMask, transitionIn, transitionOut, speed);
        }

        public void StopGesture(float transitionOut = 0.0f)
        {
            characterAnimation.StopGesture(transitionOut);
        }

        public void SetState(CharacterState state, AvatarMask avatarMask,
            float weight, float time, float speed, CharacterAnimation.Layer layer)
        {
            characterAnimation.SetState(state, avatarMask, weight, time, speed, (int) layer);
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float weight, float time, float speed,
            CharacterAnimation.Layer layer, bool syncTime = false)
        {
            characterAnimation.SetState(rtc, avatarMask, weight, time, speed, (int) layer, syncTime);
        }

        public void SetState(AnimationClip clip, AvatarMask avatarMask,
            float weight, float time, float speed, CharacterAnimation.Layer layer)
        {
            characterAnimation.SetState(clip, avatarMask, weight, time, speed, (int) layer);
        }

        public void ResetState(float time, CharacterAnimation.Layer layer)
        {
            characterAnimation.ResetState(time, (int) layer);
        }

        public void ChangeStateWeight(CharacterAnimation.Layer layer, float weight)
        {
            characterAnimation.ChangeStateWeight((int) layer, weight);
        }

        public void ResetControllerTopology(RuntimeAnimatorController runtimeController)
        {
            characterAnimation.ChangeRuntimeController(runtimeController);
        }

        public CharacterAttachments GetCharacterAttachments()
        {
            return characterAttachments;
        }

        public void SetCharacterAttachments(CharacterAttachments attachments)
        {
            characterAttachments = attachments;
        }

        public CharacterHandIK GetCharacterHandIK()
        {
            return handIK;
        }

        public CharacterState GetState(CharacterAnimation.Layer layer)
        {
            return characterAnimation.GetState((int) layer);
        }

        public void ChangeModel(GameObject prefabModel)
        {
            RuntimeAnimatorController runtimeController = null;
            Dictionary<HumanBodyBones, List<CharacterAttachments.Attachment>> attachments =
                new Dictionary<HumanBodyBones, List<CharacterAttachments.Attachment>>();

            if (characterAttachments != null)
            {
                attachments = characterAttachments.attachments;
            }

            if (animator != null)
            {
                runtimeController = animator.runtimeAnimatorController;
                Destroy(animator.gameObject);
            }

            GameObject instance = Instantiate(prefabModel, transform);
            instance.name = prefabModel.name;

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            Animator instanceAnimator = instance.GetComponent<Animator>();
            if (instanceAnimator != null)
            {
                animator = instanceAnimator;
                animator.applyRootMotion = false;
                ResetControllerTopology(runtimeController);
            }

            if (autoInitializeRagdoll)
            {
                character.InitializeRagdoll();
            }

            GenerateCharacterAttachments();
            foreach (KeyValuePair<HumanBodyBones, List<CharacterAttachments.Attachment>> item in attachments)
            {
                List<CharacterAttachments.Attachment> list = new List<CharacterAttachments.Attachment>(item.Value);
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].prefab == null) continue;
                    characterAttachments.Attach(
                        item.Key, list[i].prefab,
                        list[i].locPosition,
                        list[i].locRotation
                    );
                }
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            characterRotation.SetQuaternion(rotation);
        }

        public void SetRotationPitch(float value)
        {
            characterRotation.SetPitch(value);
        }

        public void SetRotationYaw(float value)
        {
            characterRotation.SetYaw(value);
        }

        public void SetRotationRoll(float value)
        {
            characterRotation.SetRoll(value);
        }

        public Quaternion GetCurrentRotation()
        {
            return characterRotation.GetCurrentRotation();
        }

        public Quaternion GetTargetRotation()
        {
            return characterRotation.GetTargetRotation();
        }

        public void SetVisibility(bool visible)
        {
            animator.gameObject.SetActive(visible);
        }

        public void SetStiffBody(bool stiffBody)
        {
            this.stiffBody = stiffBody;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnLand(float verticalSpeed)
        {
            if (!useProceduralLanding) return;

            float force = Mathf.InverseLerp(0f, MAX_LAND_FORCE_SPEED, verticalSpeed);
            paramValues[HASH_LAND_FORCE].Set(force);
            animator.SetTrigger(HASH_LAND);
        }

        private void GenerateAnimatorEvents()
        {
            animEvents = animator.gameObject.AddComponent<CharacterAnimatorEvents>();
            animEvents.Setup(character);
        }

        private void GenerateCharacterAttachments()
        {
            characterAttachments = animator.gameObject.AddComponent<CharacterAttachments>();
            characterAttachments.Setup(animator);
        }

        private void GenerateFootIK()
        {
            if (animator != null && animator.isHuman)
            {
                footIK = animator.gameObject.AddComponent<CharacterFootIK>();
                footIK.Setup(character);
            }
        }

        private void GenerateHandIK()
        {
            if (animator != null && animator.isHuman)
            {
                handIK = animator.gameObject.AddComponent<CharacterHandIK>();
                handIK.Setup(character);
            }
        }
    }
}