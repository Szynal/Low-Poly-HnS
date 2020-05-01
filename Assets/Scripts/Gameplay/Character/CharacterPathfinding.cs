using UnityEngine;
using UnityEngine.AI;

namespace LowPolyHnS
{
    public class CharacterPathfinding : MonoBehaviour
    {
        public Transform goalPoint;
        private RaycastHit hitInfo;
        private NavMeshAgent agent;
        private CharacterMovement movement;


        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            movement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && movement.MouseTimer < 0.3f)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                {
                    //Debug.Log(hitInfo.point);
                    agent.destination = hitInfo.point;
                }
            }
        }
    }
}