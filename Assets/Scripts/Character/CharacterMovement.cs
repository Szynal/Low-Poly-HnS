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
        private NavMeshAgent agent;
        private CharacterController controller;

        private Vector3 cursorPosition;
        private Vector3 motion = Vector3.zero;

        private Transform playerCamera;

        private Plane plane;
        private Ray ray;

        private CharacterAnimatorManger animatorManger;
        private bool isMoving = true;

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

            motion = Input.GetMouseButton(0) ? GetCursorDirection() : Vector3.zero;

            Rotate();
            controller.Move(Vector3.down);
            UpdateNavAgentPosition();

            if (animatorManger != null)
            {
                animatorManger.AnimateCharacterMovement(isMoving, motion);
            }
        }

        private Vector3 GetCursorDirection()
        {
            plane = new Plane(Vector3.up, transform.position);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var point))
            {
                cursorPosition = ray.GetPoint(point);
            }

            Vector3 heading = cursorPosition - transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            return new Vector3(direction.x, direction.z);
        }

        private void Rotate()
        {
            if (playerCamera == null) return;

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