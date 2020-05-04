using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class CharacterFootIK : MonoBehaviour
    {
        private const float FOOT_OFFSET_Y = 0.1f;
        private const float SMOOTH_POSITION = 0.1f;
        private const float SMOOTH_ROTATION = 0.1f;
        private const float SMOOTH_WEIGHT = 0.2f;
        private const float BODY_MAX_INCLINE = 10f;

        private static readonly int IK_L_FOOT = Animator.StringToHash("IK_leftFoot");
        private static readonly int IK_R_FOOT = Animator.StringToHash("IK_rightFoot");

        private class Foot
        {
            public bool hit;
            public int weightID;
            public AvatarIKGoal footIK;
            public Transform foot;

            public float height;
            public Vector3 normal = Vector3.up;

            public Foot(Transform foot, AvatarIKGoal footIK, int weightID)
            {
                hit = false;
                this.weightID = weightID;
                this.footIK = footIK;
                this.foot = foot;
            }

            public float GetWeight(Animator animator)
            {
                return animator.GetFloat(weightID);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        private Character character;
        private CharacterAnimator characterAnimator;
        private CharacterController controller;

        private Foot leftFoot;
        private Foot rightFoot;

        private float defaultOffset;
        private float speedPosition;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            this.character = character;
            characterAnimator = this.character.GetCharacterAnimator();
            animator = characterAnimator.animator;
            controller = gameObject.GetComponentInParent<CharacterController>();
            if (animator == null || !animator.isHuman || controller == null) return;

            Transform lFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

            leftFoot = new Foot(lFoot, AvatarIKGoal.LeftFoot, IK_L_FOOT);
            rightFoot = new Foot(rFoot, AvatarIKGoal.RightFoot, IK_R_FOOT);

            defaultOffset = transform.localPosition.y;
        }

        private void LateUpdate()
        {
            if (character == null || characterAnimator == null) return;
            if (!characterAnimator.useFootIK) return;
            if (character.IsRagdoll()) return;

            WeightCompensationPosition();
        }

        // IK METHODS: ----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null || !animator.isHuman) return;
            if (character == null || characterAnimator == null) return;
            if (character.IsRagdoll()) return;

            eventBeforeIK.Invoke(layerIndex);

            if (!characterAnimator.useFootIK) return;

            if (controller.isGrounded)
            {
                UpdateFoot(leftFoot);
                UpdateFoot(rightFoot);

                SetFoot(leftFoot);
                SetFoot(rightFoot);
            }

            eventAfterIK.Invoke(layerIndex);
        }

        private void UpdateFoot(Foot foot)
        {
            float rayMagnitude = controller.height / 2.0f;
            Vector3 rayPosition = foot.foot.position;
            rayPosition.y += rayMagnitude / 2.0f;

            int layerMask = characterAnimator.footLayerMask;
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

            int hitCount = Physics.RaycastNonAlloc(
                rayPosition, -Vector3.up, hitBuffer,
                rayMagnitude, layerMask, queryTrigger
            );

            if (hitCount > 0)
            {
                foot.hit = true;
                foot.height = hitBuffer[0].point.y;
                foot.normal = hitBuffer[0].normal;
            }
            else
            {
                foot.hit = false;
                foot.height = foot.foot.position.y;
            }
        }

        private void SetFoot(Foot foot)
        {
            float weight = foot.GetWeight(animator);

            if (foot.hit)
            {
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, foot.normal);
                float angle = Vector3.Angle(transform.up, foot.normal);
                Quaternion rotation = Quaternion.AngleAxis(angle * weight, rotationAxis);

                animator.SetIKRotationWeight(foot.footIK, weight);
                animator.SetIKRotation(foot.footIK, rotation * animator.GetIKRotation(foot.footIK));

                float baseHeight = transform.position.y - FOOT_OFFSET_Y;
                float animHeight = (foot.foot.position.y - baseHeight) / (rotation * Vector3.up).y;
                Vector3 position = new Vector3(
                    foot.foot.position.x,
                    Mathf.Max(foot.height, baseHeight) + animHeight,
                    foot.foot.position.z
                );

                animator.SetIKPositionWeight(foot.footIK, weight);
                animator.SetIKPosition(foot.footIK, position);
            }
            else
            {
                animator.SetIKPositionWeight(foot.footIK, weight);
                animator.SetIKRotationWeight(foot.footIK, weight);
            }
        }

        // WEIGHT COMPENSATION: -------------------------------------------------------------------

        private void WeightCompensationPosition()
        {
            float position = controller.transform.position.y + defaultOffset;

            if (controller.isGrounded)
            {
                float targetHeight = transform.position.y;

                if (leftFoot.hit && leftFoot.height < targetHeight) targetHeight = leftFoot.height;
                if (rightFoot.hit && rightFoot.height < targetHeight) targetHeight = rightFoot.height;

                targetHeight += FOOT_OFFSET_Y;
                if (position > targetHeight)
                {
                    float maxDistance = controller.transform.position.y + defaultOffset;
                    maxDistance -= controller.height * 0.075f;
                    position = Mathf.Max(targetHeight, maxDistance);
                }
            }

            float yAxis = Mathf.SmoothDamp(
                transform.position.y,
                position,
                ref speedPosition,
                SMOOTH_POSITION
            );

            transform.position = new Vector3(transform.position.x, yAxis, transform.position.z);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetControllerBase()
        {
            Vector3 position = controller.transform.TransformPoint(controller.center);
            position.y -= controller.height * 0.5f - controller.radius;

            return position;
        }
    }
}