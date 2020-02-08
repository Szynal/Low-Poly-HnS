using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace LowPolyHnS
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private StandaloneInputModule inputModule = null;
        
        private CharacterAnimatorManger animatorManger = null;
        private NavMeshAgent agent;
        private CharacterController controller;
        private Vector3 motion = Vector3.zero;
        private bool isMoving = true;
        private Transform playerCamera;
        
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animatorManger = GetComponent<CharacterAnimatorManger>();

            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (inputModule == null || playerCamera == null)
            {
                return;
            }

            motion = new Vector3(Input.GetAxis(inputModule.horizontalAxis), Input.GetAxis(inputModule.verticalAxis));
            Rotate();
            controller.Move(Vector3.down);
            UpdateNavAgentPosition();
            
            if (animatorManger != null)
            {
                animatorManger.AnimateCharacterMovement(isMoving, motion);
            }
        }

        private void Rotate()
        {
            Vector3 movementVector = playerCamera.TransformDirection(motion);

            if (movementVector == Vector3.zero) return;
            movementVector.y = 0f;
            movementVector.Normalize();
            transform.forward = Vector3.Lerp(transform.forward, movementVector, 8 * Time.deltaTime);
        }

        private void UpdateNavAgentPosition()
        {
            transform.position = new Vector3(transform.position.x, agent.nextPosition.y, transform.position.z);
            agent.nextPosition = transform.position;
        }
    }

}