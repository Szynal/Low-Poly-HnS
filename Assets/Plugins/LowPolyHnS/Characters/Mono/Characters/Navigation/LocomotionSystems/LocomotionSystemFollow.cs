using UnityEngine;
using UnityEngine.AI;

namespace LowPolyHnS.Characters
{
    public class LocomotionSystemFollow : ILocomotionSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private bool isFollowing;
        private bool usingNavmesh;
        private NavMeshPath path;

        private Transform targetTransform;
        private float minRadius;
        private float maxRadius;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();

            Vector3 currPosition = characterLocomotion.character.transform.position;
            float distance = targetTransform != null
                ? Vector3.Distance(currPosition, targetTransform.position)
                : -1.0f;

            bool stopConditions = targetTransform == null;
            stopConditions |= isFollowing && distance <= minRadius;
            stopConditions |= !isFollowing && distance <= maxRadius;

            if (stopConditions)
            {
                isFollowing = false;

                if (usingNavmesh)
                {
                    characterLocomotion.navmeshAgent.enabled = true;
                    characterLocomotion.navmeshAgent.isStopped = true;
                }
                else
                {
                    Vector3 defaultDirection = Vector3.up * characterLocomotion.verticalSpeed * Time.deltaTime;
                    characterLocomotion.characterController.Move(defaultDirection);
                }

                return usingNavmesh
                    ? CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent
                    : CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
            }

            isFollowing = true;

            if (usingNavmesh)
            {
                NavMeshAgent agent = characterLocomotion.navmeshAgent;
                agent.enabled = true;
                agent.updatePosition = true;
                agent.updateUpAxis = true;

                CharacterController controller = characterLocomotion.characterController;

                NavMeshHit hit = new NavMeshHit();
                NavMesh.SamplePosition(targetTransform.position, out hit, 1.0f, NavMesh.AllAreas);
                if (hit.hit) agent.SetDestination(hit.position);

                float remainingDistance = agent.remainingDistance;
                bool isGrounded = agent.isOnOffMeshLink;
                agent.speed = CalculateSpeed(controller.transform.forward, isGrounded);
                agent.angularSpeed = characterLocomotion.angularSpeed;

                agent.isStopped = false;
                agent.updateRotation = true;

                UpdateNavmeshAnimationConstraints();
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
            }
            else
            {
                if (characterLocomotion.navmeshAgent != null)
                {
                    characterLocomotion.navmeshAgent.enabled = false;
                }

                CharacterController controller = characterLocomotion.characterController;
                Vector3 targetPosition = Vector3.Scale(targetTransform.position, HORIZONTAL_PLANE);
                targetPosition += Vector3.up * currPosition.y;
                Vector3 targetDirection = (targetPosition - currPosition).normalized;

                float speed = CalculateSpeed(targetDirection, controller.isGrounded);
                Quaternion targetRotation = UpdateRotation(targetDirection);

                UpdateAnimationConstraints(ref targetDirection, ref targetRotation);

                targetDirection = Vector3.Scale(targetDirection, HORIZONTAL_PLANE) * speed;
                targetDirection += Vector3.up * characterLocomotion.verticalSpeed;

                controller.Move(targetDirection * Time.deltaTime);
                controller.transform.rotation = targetRotation;

                if (characterLocomotion.navmeshAgent != null && characterLocomotion.navmeshAgent.isOnNavMesh)
                {
                    Vector3 position = characterLocomotion.characterController.transform.position;
                    characterLocomotion.navmeshAgent.Warp(position);
                }

                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
            }
        }

        public override void OnDestroy()
        {
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateNavmeshAnimationConstraints()
        {
            NavMeshAgent agent = characterLocomotion.navmeshAgent;
            if (characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT)
            {
                if (agent.velocity == Vector3.zero)
                {
                    agent.Move(agent.transform.forward * agent.speed * Time.deltaTime);
                }
            }

            if (characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION)
            {
                agent.isStopped = true;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetFollow(Transform targetTransform, float minRadius, float maxRadius)
        {
            usingNavmesh = characterLocomotion.canUseNavigationMesh;
            this.targetTransform = targetTransform;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            isFollowing = false;
        }
    }
}