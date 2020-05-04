using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    public class LocomotionSystemTarget : ILocomotionSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private bool move;
        private bool usingNavmesh;
        private NavMeshPath path;

        private Vector3 targetPosition;
        private TargetRotation targetRotation;

        private float stopThreshold = STOP_THRESHOLD;
        private UnityAction onFinishCallback;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();
            if (!move)
            {
                if (usingNavmesh)
                {
                    characterLocomotion.navmeshAgent.enabled = false;
                    usingNavmesh = false;
                }

                Vector3 defaultDirection = Vector3.up * characterLocomotion.verticalSpeed;
                characterLocomotion.characterController.Move(defaultDirection * Time.deltaTime);

                Transform characterTransform = characterLocomotion.character.transform;
                Vector3 forward = characterTransform.TransformDirection(Vector3.forward);

                Quaternion rotation = UpdateRotation(forward);
                characterLocomotion.character.transform.rotation = rotation;

                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;

                /*
                if (!this.usingNavmesh)
                {
					Vector3 defaultDirection = Vector3.up * this.characterLocomotion.verticalSpeed;
                    this.characterLocomotion.characterController.Move(defaultDirection * Time.deltaTime);
                    return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
                }

                this.characterLocomotion.navmeshAgent.enabled = true;
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
                */
            }

            if (usingNavmesh)
            {
                NavMeshAgent agent = characterLocomotion.navmeshAgent;
                agent.enabled = true;

                CharacterController controller = characterLocomotion.characterController;
                if (agent.pathPending) return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;

                if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    float distance = Mathf.Min(
                        Vector3.Distance(agent.pathEndPosition, agent.transform.position),
                        agent.remainingDistance
                    );

                    if (!agent.hasPath && distance < STOP_THRESHOLD)
                    {
                        Stopping();
                    }

                    return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
                }


                float remainingDistance = agent.remainingDistance;
                bool isGrounded = agent.isOnOffMeshLink;
                agent.speed = CalculateSpeed(controller.transform.forward, isGrounded);
                agent.angularSpeed = characterLocomotion.angularSpeed;

                agent.isStopped = false;
                agent.updateRotation = true;

                if (remainingDistance <= stopThreshold)
                {
                    agent.updateRotation = true;
                    Stopping();
                }
                else if (remainingDistance <= stopThreshold + SLOW_THRESHOLD)
                {
                    Slowing(remainingDistance);
                }
                else
                {
                    Moving();
                }

                UpdateNavmeshAnimationConstraints();
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
            }
            else
            {
                if (characterLocomotion.navmeshAgent != null &&
                    characterLocomotion.navmeshAgent.enabled)
                {
                    characterLocomotion.navmeshAgent.enabled = false;
                }

                CharacterController controller = characterLocomotion.characterController;
                Vector3 targetPos = Vector3.Scale(targetPosition, HORIZONTAL_PLANE);
                targetPos += Vector3.up * controller.transform.position.y;
                Vector3 targetDirection = (targetPos - controller.transform.position).normalized;

                float speed = CalculateSpeed(targetDirection, controller.isGrounded);
                Quaternion targetRot = UpdateRotation(targetDirection);

                UpdateAnimationConstraints(ref targetDirection, ref targetRot);

                targetDirection = Vector3.Scale(targetDirection, HORIZONTAL_PLANE) * speed;
                targetDirection += Vector3.up * characterLocomotion.verticalSpeed;

                controller.Move(targetDirection * Time.deltaTime);
                controller.transform.rotation = targetRot;

                float remainingDistance = Vector3.Distance(
                    Vector3.Scale(controller.transform.position, HORIZONTAL_PLANE),
                    Vector3.Scale(targetPosition, HORIZONTAL_PLANE)
                );

                if (remainingDistance <= stopThreshold)
                {
                    Stopping();
                }
                else if (remainingDistance <= stopThreshold + SLOW_THRESHOLD)
                {
                    Slowing(remainingDistance);
                }

                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
            }
        }

        public override void OnDestroy()
        {
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Stopping()
        {
            if (characterLocomotion.navmeshAgent != null &&
                characterLocomotion.navmeshAgent.enabled)
            {
                characterLocomotion.navmeshAgent.isStopped = true;
            }

            FinishMovement();
            move = false;

            if (targetRotation.hasRotation &&
                characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                characterLocomotion.character.transform.rotation = targetRotation.rotation;
            }
        }

        private void Slowing(float distanceToDestination)
        {
            float tDistance = 1f - distanceToDestination / (stopThreshold + SLOW_THRESHOLD);

            Transform characterTransform = characterLocomotion.character.transform;
            Quaternion desiredRotation = UpdateRotation(characterTransform.TransformDirection(Vector3.forward));

            if (targetRotation.hasRotation &&
                characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                desiredRotation = targetRotation.rotation;
            }

            characterTransform.rotation = Quaternion.Lerp(
                characterTransform.rotation,
                desiredRotation,
                tDistance
            );
        }

        private void Moving()
        {
            Quaternion desiredRotation = UpdateRotation(
                characterLocomotion.navmeshAgent.desiredVelocity
            );

            characterLocomotion.character.transform.rotation = desiredRotation;
        }

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

        private void FinishMovement()
        {
            if (onFinishCallback != null)
            {
                onFinishCallback.Invoke();
                onFinishCallback = null;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        public void SetTarget(Ray ray, LayerMask layerMask, TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            int hitCount = Physics.RaycastNonAlloc(
                ray, hitBuffer, Mathf.Infinity,
                layerMask, queryTrigger
            );

            if (hitCount > 0)
            {
                SetTarget(hitBuffer[0].point, rotation, stopThreshold, callback);
            }
        }

        public void SetTarget(Vector3 position, TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            move = true;
            usingNavmesh = false;

            this.stopThreshold = Mathf.Max(stopThreshold, STOP_THRESHOLD);
            onFinishCallback = callback;

            if (characterLocomotion.canUseNavigationMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas)) position = hit.position;

                path = new NavMeshPath();
                bool pathFound = NavMesh.CalculatePath(
                    characterLocomotion.characterController.transform.position,
                    position,
                    NavMesh.AllAreas,
                    path
                );

                if (pathFound)
                {
                    Debug.DrawLine(position, position + Vector3.up, Color.green, 0.1f);

                    usingNavmesh = true;
                    characterLocomotion.navmeshAgent.enabled = true;

                    characterLocomotion.navmeshAgent.updatePosition = true;
                    characterLocomotion.navmeshAgent.updateUpAxis = true;

                    characterLocomotion.navmeshAgent.isStopped = false;
                    characterLocomotion.navmeshAgent.SetPath(path);
                }
            }

            targetPosition = position;
            targetRotation = rotation ?? new TargetRotation();
        }

        public void Stop(TargetRotation rotation = null, UnityAction callback = null)
        {
            SetTarget(
                characterLocomotion.characterController.transform.position,
                rotation,
                0f,
                callback
            );
        }
    }
}