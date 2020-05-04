using UnityEngine;

namespace LowPolyHnS.Characters
{
    public class LocomotionSystemTank : ILocomotionSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected Vector3 desiredDirection = Vector3.zero;
        protected float rotationY;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();

            if (characterLocomotion.navmeshAgent != null)
            {
                characterLocomotion.navmeshAgent.updatePosition = false;
                characterLocomotion.navmeshAgent.updateUpAxis = false;
            }

            Vector3 targetDirection = desiredDirection;
            Quaternion targetRotation = Quaternion.identity;

            CharacterController controller = characterLocomotion.characterController;

            float targetSpeed = CalculateSpeed(targetDirection, controller.isGrounded);

            UpdateAnimationConstraints(ref targetDirection, ref targetRotation);
            targetDirection *= targetSpeed;

            pivotSpeed = rotationY * characterLocomotion.angularSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(Vector3.up * pivotSpeed);

            UpdateSliding();

            if (isSliding) targetDirection = slideDirection;

            if (isRootMoving)
            {
                characterLocomotion.characterController.transform.rotation = targetRotation;
            }
            else if (isDashing)
            {
                targetDirection = dashVelocity;
                targetRotation = controller.transform.rotation;

                controller.Move(targetDirection * Time.deltaTime);
                controller.transform.rotation *= targetRotation;
            }
            else
            {
                controller.Move(targetDirection * Time.deltaTime);
                controller.transform.rotation *= targetRotation;
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

        public void SetDirection(Vector3 direction, float rotationY)
        {
            desiredDirection = direction;
            this.rotationY = rotationY;
        }
    }
}