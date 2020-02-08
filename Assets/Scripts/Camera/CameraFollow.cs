using UnityEngine;

namespace LowPolyHnS
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        public bool IsPlayersCamera = false;
        public Vector3 Offset;
        
        private void Start()
        {
            if (followTarget == null && IsPlayersCamera == true)
            {
                followTarget = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        private void LateUpdate()
        {
            Follow();
        }

        private void Follow()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 targetPosition = followTarget.position + Offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 3 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(followTarget.transform.position - transform.position), Time.deltaTime * 2);
        }
    }
}