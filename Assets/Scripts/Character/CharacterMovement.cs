using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

namespace LowPolyHnS
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        private NavMeshAgent agent;
        private CharacterController controller;

        private Vector3 cursorPosition;
        private Vector3 motion = Vector3.zero;

        private Transform playerCamera;

        private Plane plane;
        private Ray ray;

        private CharacterAnimatorManger animatorManger;

        private bool canMove = true;
        private bool isMoving = true;
        

        public float MouseTimer = 0f;
        [SerializeField] private float mouseClickTime = 0.1f;

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
            if (Input.GetMouseButtonDown(0))
            {
                MouseTimer = 0f;
            }

            if (Input.GetMouseButton(0))
            {
                MouseTimer += Time.deltaTime;
            }

            if (canMove)
            {
                Move();
            }
        }

        private void Move()
        {
            if ( playerCamera == null)
            {
                return;
            }

            if (MouseTimer < mouseClickTime)
            {
                motion = agent.hasPath ? GetNextPointDirection() : Input.GetMouseButton(0) ? GetCursorDirection() : Vector3.zero;
            }else if (MouseTimer > mouseClickTime)
            {
                agent.ResetPath();
                motion = Input.GetMouseButton(0) ? GetCursorDirection() : Vector3.zero;
            }
            

            UpdateNavAgentPosition2();
            Rotate();
            controller?.Move(Vector3.down);

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

        private Vector3 GetNextPointDirection()
        {
            
            Vector3 heading = agent.steeringTarget - transform.position;
            float distance = heading.magnitude;

            Vector3 direction = heading / distance;
            if ( Vector3.Magnitude(agent.pathEndPosition - transform.position) < 0.3f)
            {
                agent.ResetPath();
            }
            return new  Vector3(direction.x, direction.z);
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
        private void UpdateNavAgentPosition2()
        {
            agent.nextPosition = transform.position;
        }

        public async void EnableRagdoll(float delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            if (controller != null) controller.enabled = false;
            if (animatorManger != null) animatorManger.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            canMove = false;
        }
    }
}