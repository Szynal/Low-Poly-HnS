using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [Serializable]
    public class CharacterRagdoll
    {
        public enum State
        {
            Normal,
            Ragdoll,
            Recover
        }

        public enum Bone
        {
            Root,
            L_Hip,
            L_Knee,
            R_Hip,
            R_Knee,
            Spine,
            L_Arm,
            L_Elbow,
            R_Arm,
            R_Elbow,
            Head,
            Character
        }

        public class BoneData
        {
            public string name;

            public Transform anchor;
            public Joint joint;
            public Rigidbody rigidbody;
            public Collider collider;
            public Bone parent;

            public float minLimit;
            public float maxLimit;
            public float swingLimit;

            public Vector3 axis;
            public Vector3 normalAxis;

            public float radiusScale;
            public Type colliderType;

            public List<Bone> children;
            public float density;

            public BoneData()
            {
                children = new List<Bone>();
                density = 0.1f;
            }

            public BoneData(Transform bone) : this()
            {
                name = bone.gameObject.name;
                anchor = bone;
            }
        }

        private class HumanChunk
        {
            public Transform transform;
            public Vector3 localPosition;
            public Vector3 worldPosition;

            public Quaternion worldRotation;
            public Quaternion localRotation;

            public HumanChunk(Transform transform)
            {
                this.transform = transform;
                Snapshot();
            }

            public void Snapshot()
            {
                worldPosition = transform.position;
                localPosition = transform.localPosition;

                worldRotation = transform.rotation;
                localRotation = transform.localRotation;
            }
        }

        private const float SMOOTH_RAGDOLL_FOLLOW = 0.2f;
        private const float SMOOTH_RAGDOLL_TRANSITION = 0.75f;

        private static readonly Vector3 PLANE_XZ = new Vector3(1f, 0f, 1f);

        // PROPERTIES: ----------------------------------------------------------------------------

        private Character character;
        private CharacterAnimator charAnimator;

        private State state = State.Normal;
        private bool isInitialized;

        private Vector3 interpolation = Vector3.zero;
        private float changeTime = -100f;
        private float stableTime = -100f;

        private List<HumanChunk> chunks = new List<HumanChunk>();
        private HumanChunk rootChunk;

        private bool startRecover;
        private Vector3 startRecoverDirection = Vector3.zero;

        private bool autoStandUp;
        private BoneData[] bones = new BoneData[0];
        private BoneData root;

        private RaycastHit[] hitBuffer = new RaycastHit[10];

        // INITIALIZERS: --------------------------------------------------------------------------

        public CharacterRagdoll(Character character)
        {
            this.character = character;
            charAnimator = this.character.GetCharacterAnimator();

            Initialize(true);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Initialize(bool forceInitialize = false)
        {
            if (!forceInitialize && isInitialized) return;
            if (charAnimator == null || !charAnimator.animator.isHuman) return;

            bool result = BuildBonesData() &&
                          BuildColliders() &&
                          BuildBodies() &&
                          BuildJoints() &&
                          BuildMasses() &&
                          BuildLayers() &&
                          BuildChunks();

            isInitialized = result;
        }

        public void Ragdoll(bool active, bool autoStandUp)
        {
            this.autoStandUp = autoStandUp;
            changeTime = Time.time;
            stableTime = Time.time;

            for (int i = 0; i < bones.Length; ++i)
            {
                bones[i].rigidbody.isKinematic = !active;
                bones[i].collider.enabled = active;
            }

            switch (active)
            {
                case true:
                    ToRagdoll();
                    break;
                case false:
                    ToRecover();
                    break;
            }
        }

        public State GetState()
        {
            return state;
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public void Update()
        {
            switch (state)
            {
                case State.Ragdoll:
                    UpdateRagdoll();
                    break;
                case State.Recover:
                    UpdateRecover();
                    break;
            }
        }

        private void UpdateRagdoll()
        {
            character.transform.position = Vector3.SmoothDamp(
                character.transform.position,
                root.anchor.position,
                ref interpolation,
                SMOOTH_RAGDOLL_FOLLOW
            );

            Vector3 ragdollDirection = GetRagdollDirection();
            character.transform.rotation = Quaternion.LookRotation(
                ragdollDirection,
                Vector3.up
            );

            if (autoStandUp)
            {
                if (root.rigidbody.velocity.magnitude > 0.1f) stableTime = Time.time;
                if (Time.time - stableTime > 0.5f) character.SetRagdoll(false);
            }
        }

        private void UpdateRecover()
        {
            if (startRecover)
            {
                startRecover = false;
                character.transform.rotation = Quaternion.LookRotation(
                    startRecoverDirection,
                    Vector3.up
                );
            }

            float duration = SMOOTH_RAGDOLL_TRANSITION;
            float t = (Time.time - changeTime) / duration;

            rootChunk.transform.localPosition = Vector3.Lerp(
                rootChunk.localPosition,
                rootChunk.transform.localPosition,
                t
            );

            rootChunk.transform.localRotation = Quaternion.Lerp(
                rootChunk.localRotation,
                rootChunk.transform.localRotation,
                t
            );

            for (int i = 0; i < chunks.Count; ++i)
            {
                if (chunks[i].transform == root.anchor)
                {
                    chunks[i].transform.position = Vector3.Lerp(
                        chunks[i].worldPosition,
                        chunks[i].transform.position,
                        t
                    );
                }

                if (chunks[i].localRotation != chunks[i].transform.localRotation)
                {
                    chunks[i].transform.rotation = Quaternion.Lerp(
                        chunks[i].worldRotation,
                        chunks[i].transform.rotation,
                        t
                    );
                }
            }

            if (t >= 1f)
            {
                state = State.Normal;
                character.characterLocomotion.isBusy = false;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ToRagdoll()
        {
            rootChunk.Snapshot();
            character.characterLocomotion.isBusy = true;

            state = State.Ragdoll;
            interpolation = Vector3.zero;

            Vector3 velocity = character.characterLocomotion.characterController.velocity;
            for (int i = 0; i < bones.Length; ++i)
            {
                bones[i].rigidbody.AddForce(velocity, ForceMode.VelocityChange);
            }
        }

        private void ToRecover()
        {
            state = State.Recover;
            startRecover = true;

            for (int i = 0; i < chunks.Count; ++i)
            {
                chunks[i].Snapshot();
            }

            int hitCount = Physics.RaycastNonAlloc(root.anchor.position, Vector3.down, hitBuffer, 5f);
            for (int i = 0; i < hitCount; ++i)
            {
                if (!hitBuffer[i].transform.IsChildOf(character.transform) &&
                    !hitBuffer[i].transform.IsChildOf(root.anchor))
                {
                    float offset = character.characterLocomotion.characterController.skinWidth;
                    character.transform.position = hitBuffer[i].point + Vector3.up * offset;
                    break;
                }
            }

            Vector3 ragdollDirection = Vector3.zero;
            switch (root.anchor.forward.y < 0f)
            {
                case true:
                    charAnimator.CrossFadeGesture(charAnimator.standFaceDown, 1f, null, 0f, 0.25f);
                    ragdollDirection = GetRagdollDirection();
                    break;

                case false:
                    charAnimator.CrossFadeGesture(charAnimator.standFaceUp, 1f, null, 0f, 0.25f);
                    ragdollDirection = -1f * GetRagdollDirection();
                    break;
            }

            startRecoverDirection = ragdollDirection;
        }

        private Vector3 GetRagdollDirection()
        {
            Vector3 pointLKnee = bones[(int) Bone.L_Knee].anchor.position;
            Vector3 pointRKnee = bones[(int) Bone.R_Knee].anchor.position;
            Vector3 pointFeet = (pointLKnee + pointRKnee) / 2.0f;
            Vector3 pointHead = bones[(int) Bone.Head].anchor.position;

            Vector3 direction = Vector3.Scale(pointHead - pointFeet, PLANE_XZ);

            return direction;
        }

        // BUILD METHODS: -------------------------------------------------------------------------

        private bool BuildBonesData()
        {
            Animator animator = charAnimator.animator;
            if (animator == null) return false;

            bones = new[]
            {
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Hips)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Spine)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightUpperArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.RightLowerArm)),
                new BoneData(animator.GetBoneTransform(HumanBodyBones.Head))
            };

            root = bones[(int) Bone.Root];

            Vector3 unitX = root.anchor.TransformDirection(Vector3.right);
            Vector3 unitY = root.anchor.TransformDirection(Vector3.up);
            Vector3 unitZ = root.anchor.TransformDirection(Vector3.forward);

            SetupJoint(Bone.L_Hip, Bone.Root, unitX, unitZ, -20, 70, 30, typeof(CapsuleCollider), 0.3f, 0.1f);
            SetupJoint(Bone.R_Hip, Bone.Root, unitX, unitZ, -20, 70, 30, typeof(CapsuleCollider), 0.3f, 0.1f);

            SetupJoint(Bone.L_Knee, Bone.L_Hip, unitX, unitZ, -80, 0, 0, typeof(CapsuleCollider), 0.25f, 0.05f);
            SetupJoint(Bone.R_Knee, Bone.R_Hip, unitX, unitZ, -80, 0, 0, typeof(CapsuleCollider), 0.25f, 0.05f);

            SetupJoint(Bone.Spine, Bone.Root, unitX, unitZ, -20, 20, 10, typeof(BoxCollider), 0.25f, 0.2f);
            SetupJoint(Bone.Head, Bone.Spine, unitX, unitZ, -40, 25, 25, typeof(SphereCollider), 1f, 0.1f);

            SetupJoint(Bone.L_Arm, Bone.Spine, unitY, unitZ, -70, 10, 50, typeof(CapsuleCollider), 0.25f, 0.075f);
            SetupJoint(Bone.R_Arm, Bone.Spine, unitY, unitZ, -70, 10, 50, typeof(CapsuleCollider), 0.25f, 0.075f);

            SetupJoint(Bone.L_Elbow, Bone.L_Arm, unitZ, unitY, -90, 0, 0, typeof(CapsuleCollider), 0.25f, 0.075f);
            SetupJoint(Bone.R_Elbow, Bone.R_Arm, unitZ, unitY, -90, 0, 0, typeof(CapsuleCollider), 0.20f, 0.075f);

            return true;
        }

        private void SetupJoint(Bone bone, Bone parent, Vector3 twistAxis, Vector3 swingAxis,
            float minLimit, float maxLimit, float swingLimit, Type collider,
            float radius, float density)
        {
            int boneIndex = (int) bone;

            bones[boneIndex].axis = twistAxis;
            bones[boneIndex].normalAxis = swingAxis;

            bones[boneIndex].minLimit = minLimit;
            bones[boneIndex].maxLimit = maxLimit;
            bones[boneIndex].swingLimit = swingLimit;

            bones[boneIndex].colliderType = collider;
            bones[boneIndex].radiusScale = radius;
            bones[boneIndex].density = density;

            bones[boneIndex].parent = parent;
            bones[(int) parent].children.Add(bone);
        }

        private bool BuildColliders()
        {
            BuildColliderCapsule(bones[(int) Bone.L_Hip]);
            BuildColliderCapsule(bones[(int) Bone.R_Hip]);
            BuildColliderCapsule(bones[(int) Bone.L_Knee]);
            BuildColliderCapsule(bones[(int) Bone.R_Knee]);

            BuildColliderCapsule(bones[(int) Bone.L_Arm]);
            BuildColliderCapsule(bones[(int) Bone.R_Arm]);
            BuildColliderCapsule(bones[(int) Bone.L_Elbow]);
            BuildColliderCapsule(bones[(int) Bone.R_Elbow]);

            BuildColliderBox(bones[(int) Bone.Spine], false);
            BuildColliderBox(bones[(int) Bone.Root], true);
            BuildColliderSphere(bones[(int) Bone.Head]);

            return true;
        }

        private void BuildColliderCapsule(BoneData bone)
        {
            int direction = 0;
            float distance = 0.0f;

            if (bone.children.Count == 1)
            {
                Vector3 endPoint = bones[(int) bone.children[0]].anchor.position;
                RagdollUtilities.GetDirection(
                    bone.anchor.InverseTransformPoint(endPoint),
                    out direction, out distance
                );
            }
            else
            {
                Vector3 length = bone.anchor.position - bones[(int) bone.parent].anchor.position;
                Vector3 endPoint = bone.anchor.position + length;

                RagdollUtilities.GetDirection(
                    bone.anchor.InverseTransformPoint(endPoint),
                    out direction, out distance
                );

                if (bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                {
                    Bounds bounds = RagdollUtilities.GetBounds(
                        bone.anchor,
                        bone.anchor.GetComponentsInChildren<Transform>()
                    );

                    if (distance > 0f) distance = bounds.max[direction];
                    if (distance < 0f) distance = bounds.min[direction];
                }
            }

            CapsuleCollider capsuleCollider = bone.anchor.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.enabled = false;
            capsuleCollider.direction = direction;
            bone.collider = capsuleCollider;

            Vector3 center = Vector3.zero;
            center[direction] = distance / 2f;
            capsuleCollider.center = center;
            capsuleCollider.height = Mathf.Abs(distance);
            capsuleCollider.radius = Mathf.Abs(distance * bone.radiusScale);
        }

        private void BuildColliderSphere(BoneData bone)
        {
            float radius = 0.25f * Vector3.Distance(
                               bones[(int) Bone.L_Arm].anchor.position,
                               bones[(int) Bone.R_Arm].anchor.position
                           );

            SphereCollider sphereCollider = bone.anchor.gameObject.AddComponent<SphereCollider>();
            sphereCollider.enabled = false;
            sphereCollider.radius = radius;
            bone.collider = sphereCollider;

            Vector3 center = Vector3.zero;
            int direction = 0;
            float distance = 0.0f;

            Vector3 point = bone.anchor.InverseTransformPoint(bones[(int) Bone.Root].anchor.position);
            RagdollUtilities.GetDirection(point, out direction, out distance);

            center[direction] = Mathf.Sign(-distance) * radius;
            sphereCollider.center = center;
        }

        private void BuildColliderBox(BoneData bone, bool below)
        {
            Transform spine = bones[(int) Bone.Spine].anchor;
            Vector3 axisUp = bone.anchor.TransformDirection(Vector3.up);

            Transform[] limbs =
            {
                bones[(int) Bone.L_Hip].anchor,
                bones[(int) Bone.R_Hip].anchor,
                bones[(int) Bone.L_Arm].anchor,
                bones[(int) Bone.R_Arm].anchor
            };

            Bounds bounds = RagdollUtilities.GetBounds(bone.anchor, limbs);
            bounds = RagdollUtilities.ProportionalBounds(bounds);
            bounds = RagdollUtilities.Clip(bounds, bone.anchor, spine, axisUp, below);

            BoxCollider boxCollider = bone.anchor.gameObject.AddComponent<BoxCollider>();
            boxCollider.enabled = false;
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;
            bone.collider = boxCollider;
        }

        private bool BuildBodies()
        {
            for (int i = 0; i < bones.Length; ++i)
            {
                bones[i].rigidbody = bones[i].anchor.gameObject.AddComponent<Rigidbody>();
                bones[i].rigidbody.mass = bones[i].density;
                bones[i].rigidbody.isKinematic = true;
                bones[i].rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            return true;
        }

        private bool BuildJoints()
        {
            for (int i = 1; i < bones.Length; ++i)
            {
                CharacterJoint joint = bones[i].anchor.gameObject.AddComponent<CharacterJoint>();

                Vector3 jointAxis = bones[i].anchor.InverseTransformDirection(bones[i].axis);
                Vector3 jointNomal = bones[i].anchor.InverseTransformDirection(bones[i].normalAxis);

                joint.axis = RagdollUtilities.GetDirectionAxis(jointAxis);
                joint.swingAxis = RagdollUtilities.GetDirectionAxis(jointNomal);

                joint.anchor = Vector3.zero;
                joint.connectedBody = bones[(int) bones[i].parent].rigidbody;
                joint.enablePreprocessing = false;

                SoftJointLimit limit = new SoftJointLimit();
                limit.contactDistance = 0;

                limit.limit = bones[i].minLimit;
                joint.lowTwistLimit = limit;

                limit.limit = bones[i].maxLimit;
                joint.highTwistLimit = limit;

                limit.limit = bones[i].swingLimit;
                joint.swing1Limit = limit;

                limit.limit = 0;
                joint.swing2Limit = limit;

                bones[i].joint = joint;
            }

            return true;
        }

        private bool BuildMasses()
        {
            for (int i = 0; i < bones.Length; ++i)
            {
                bones[i].rigidbody.mass *= charAnimator.ragdollMass;
            }

            return true;
        }

        private bool BuildLayers()
        {
            for (int i = 0; i < bones.Length; ++i)
            {
                bones[i].collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            return true;
        }

        private bool BuildChunks()
        {
            Transform target = charAnimator.animator.transform;
            Transform[] children = target.GetComponentsInChildren<Transform>();

            rootChunk = new HumanChunk(target);
            chunks = new List<HumanChunk>();
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] == target) continue;
                chunks.Add(new HumanChunk(children[i]));
            }

            return true;
        }

        // GIZMOS: --------------------------------------------------------------------------------

        public void OnDrawGizmos()
        {
            GizmoBone(Bone.Root, Bone.L_Hip);
            GizmoBone(Bone.Root, Bone.R_Hip);
            GizmoBone(Bone.L_Hip, Bone.L_Knee);
            GizmoBone(Bone.R_Hip, Bone.R_Knee);

            GizmoBone(Bone.Root, Bone.Spine);
            GizmoBone(Bone.Spine, Bone.Head);

            GizmoBone(Bone.Spine, Bone.L_Arm);
            GizmoBone(Bone.Spine, Bone.R_Arm);
            GizmoBone(Bone.L_Arm, Bone.L_Elbow);
            GizmoBone(Bone.R_Arm, Bone.R_Elbow);
        }

        private void GizmoBone(Bone a, Bone b)
        {
            if (bones == null || bones.Length == 0) return;
            if (bones[(int) a].anchor == null) return;
            if (bones[(int) b].anchor == null) return;

            Color tempColor = Gizmos.color;
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                bones[(int) a].anchor.position,
                bones[(int) b].anchor.position
            );

            Gizmos.color = tempColor;
        }
    }
}