using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;


namespace LowPolyHnS
{
    public class CharacterPathfinding : MonoBehaviour
    {
        public Transform goalPoint;
        RaycastHit hitInfo = new RaycastHit();
        private NavMeshAgent agent = null;
        private CharacterMovement movement = null;


        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            movement = GetComponent<CharacterMovement>();

        }

        void Update()
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

