using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class CharacterHandIK : MonoBehaviour
    {
        public enum Limb
        {
            LeftHand,
            RightHand,
            NearestHand,
            BothHands
        }

        private class Hand
        {
            public AvatarIKGoal handIK;
            public Transform hand;

            private float changeTime;
            private float currentWeight;
            private Vector3 currentPosition;

            private bool targetReach;
            private float targetTime;
            private Transform targetTransform;
            private Vector3 targetPosition;

            public Hand(Transform hand, AvatarIKGoal handIK)
            {
                this.handIK = handIK;
                this.hand = hand;

                changeTime = 0.0f;
                currentWeight = 0.0f;

                targetReach = false;
                targetTime = 1.0f;

                targetPosition = Vector3.zero;
            }

            public void Update(Animator animator)
            {
                if (targetTransform != null)
                {
                    targetPosition = targetTransform.position;
                }

                if (targetReach)
                {
                    float t = Time.time - changeTime;
                    t = Easing.QuadInOut(0.0f, 1.0f, t / targetTime);

                    currentWeight = Mathf.Lerp(
                        currentWeight,
                        1.0f,
                        t
                    );

                    currentPosition = Vector3.Slerp(
                        currentPosition,
                        targetPosition,
                        Easing.QuadInOut(0.0f, 1.0f, t / targetTime)
                    );
                }
                else
                {
                    float t = Time.time - changeTime;
                    t = Easing.QuadInOut(0.0f, 1.0f, t / targetTime);

                    currentWeight = Mathf.Lerp(
                        currentWeight,
                        0.0f,
                        t
                    );

                    currentPosition = targetPosition;
                }

                animator.SetIKPositionWeight(handIK, currentWeight);
                animator.SetIKPosition(handIK, currentPosition);
            }

            public void Reach(Animator animator, Transform targetTransform, float duration)
            {
                targetReach = true;
                targetTime = Mathf.Max(duration, 0.01f);

                this.targetTransform = targetTransform;
                targetPosition = targetTransform.position;

                currentPosition = hand.position;
                changeTime = Time.time;
            }

            public void Unreach(float duration)
            {
                targetReach = false;
                targetTime = Mathf.Max(duration, 0.01f);
                changeTime = Time.time;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        private Character character;
        private CharacterAnimator characterAnimator;
        private CharacterController controller;

        private Hand handL;
        private Hand handR;

        public CharacterAnimator.EventIK eventBeforeIK = new CharacterAnimator.EventIK();
        public CharacterAnimator.EventIK eventAfterIK = new CharacterAnimator.EventIK();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Character character)
        {
            this.character = character;
            characterAnimator = this.character.GetCharacterAnimator();
            animator = characterAnimator.animator;
            controller = gameObject.GetComponentInParent<CharacterController>();
            if (animator == null || !animator.isHuman || controller == null) return;

            Transform handLTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform handRTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);

            handL = new Hand(handLTransform, AvatarIKGoal.LeftHand);
            handR = new Hand(handRTransform, AvatarIKGoal.RightHand);
        }

        // IK METHODS: ----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null || !animator.isHuman) return;
            if (character == null || characterAnimator == null) return;
            if (character.IsRagdoll()) return;

            eventBeforeIK.Invoke(layerIndex);

            if (!characterAnimator.useHandIK) return;

            UpdateHand(handL);
            UpdateHand(handR);

            eventAfterIK.Invoke(layerIndex);
        }

        private void UpdateHand(Hand hand)
        {
            hand.Update(animator);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Reach(Limb limb, Transform target, float duration)
        {
            switch (limb)
            {
                case Limb.LeftHand:
                    handL.Reach(animator, target, duration);
                    break;

                case Limb.RightHand:
                    handR.Reach(animator, target, duration);
                    break;

                case Limb.NearestHand:
                    NearestHand(target).Reach(animator, target, duration);
                    break;

                case Limb.BothHands:
                    handL.Reach(animator, target, duration);
                    handR.Reach(animator, target, duration);
                    break;
            }
        }

        public void LetGo(Limb limb, float duration)
        {
            switch (limb)
            {
                case Limb.LeftHand:
                    handL.Unreach(duration);
                    break;

                case Limb.RightHand:
                    handR.Unreach(duration);
                    break;

                default:
                    handL.Unreach(duration);
                    handR.Unreach(duration);
                    break;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Hand NearestHand(Transform target)
        {
            Vector3 tL = handL.hand.position;
            Vector3 tR = handR.hand.position;

            return Vector3.Distance(tL, target.position) < Vector3.Distance(tR, target.position)
                ? handL
                : handR;
        }
    }
}