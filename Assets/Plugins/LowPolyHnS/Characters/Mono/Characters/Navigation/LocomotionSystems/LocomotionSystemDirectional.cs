using UnityEngine;

namespace LowPolyHnS.Characters
{
    public class LocomotionSystemDirectional : ILocomotionSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected Vector3 desiredDirection = Vector3.zero;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            if (!characterLocomotion.characterController.enabled)
            {
                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
            }

            base.Update();

            if (characterLocomotion.navmeshAgent != null)
            {
                characterLocomotion.navmeshAgent.updatePosition = false;
                characterLocomotion.navmeshAgent.updateUpAxis = false;
            }

            Vector3 targetDirection = desiredDirection;

            float speed = CalculateSpeed(targetDirection, characterLocomotion.characterController.isGrounded);
            Quaternion targetRotation = UpdateRotation(targetDirection);

            UpdateAnimationConstraints(ref targetDirection, ref targetRotation);
            UpdateSliding();

            targetDirection = Vector3.ClampMagnitude(Vector3.Scale(targetDirection, HORIZONTAL_PLANE), 1.0f);
            targetDirection *= speed;

            if (isSliding) targetDirection = slideDirection;

            if (isRootMoving)
            {
                characterLocomotion.characterController.transform.rotation = targetRotation;
            }
            else if (isDashing)
            {
                targetDirection = dashVelocity;
                targetRotation = characterLocomotion.characterController.transform.rotation;

                characterLocomotion.characterController.Move(targetDirection * Time.deltaTime);
                characterLocomotion.characterController.transform.rotation = targetRotation;
            }
            else
            {
                characterLocomotion.characterController.Move(targetDirection * Time.deltaTime);
                characterLocomotion.characterController.transform.rotation = targetRotation;
            }

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