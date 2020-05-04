using UnityEngine;

namespace LowPolyHnS.Characters
{
    public class LocomotionSystemRotation : ILocomotionSystem
    {
        private const float ERROR_MARGIN = 0.1f;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected Vector3 desiredDirection = Vector3.zero;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();

            if (characterLocomotion.navmeshAgent != null)
            {
                characterLocomotion.navmeshAgent.updatePosition = false;
                characterLocomotion.navmeshAgent.updateUpAxis = false;
            }

            Quaternion targetRotation = UpdateRotation(desiredDirection);
            Transform charTransform = characterLocomotion.characterController.transform;

            Vector3 charForward = charTransform.TransformDirection(Vector3.forward);
            Vector3 charRight = charTransform.TransformDirection(Vector3.right);

            float difference = Vector3.Dot(charForward, desiredDirection);

            if (Mathf.Abs(difference) < ERROR_MARGIN) pivotSpeed = 0f;
            else
            {
                pivotSpeed = Vector3.Dot(charRight, desiredDirection);
                if (difference < 0f) pivotSpeed = pivotSpeed >= 0 ? 1f : -1f;
            }

            characterLocomotion.characterController.transform.rotation = targetRotation;

            if (characterLocomotion.navmeshAgent != null &&
                characterLocomotion.navmeshAgent.isActiveAndEnabled)
            {
                characterLocomotion.navmeshAgent.enabled = false;
            }

            return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
        }

        public override void OnDestroy()
        {
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetDirection(Vector3 direction, TargetRotation rotation = null)
        {
            desiredDirection = direction;
        }
    }
}