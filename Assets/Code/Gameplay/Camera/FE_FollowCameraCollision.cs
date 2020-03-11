using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_FollowCameraCollision : MonoBehaviour
{
    [SerializeField] float minDistance = 0.3f;
    [SerializeField] float desiredDistance = 1.5f;
    [SerializeField] LayerMask losMask = 0;
    [SerializeField] Collider playerCollision = null;

    private void Update()
    {
        RaycastHit _hit = new RaycastHit();
        if(Physics.Raycast(transform.position, playerCollision.ClosestPoint(transform.position) - transform.position, out _hit, 3f, losMask) == true)
        {
            Debug.Log(_hit.collider.name);
            if(_hit.collider != playerCollision && distFromPlayer() > minDistance)
            {
                transform.position += transform.forward * 0.1f;
            }
            else if(_hit.collider == playerCollision && distFromPlayer() < desiredDistance)
            {
                if (Physics.OverlapSphere(transform.position, 0.2f, losMask).Length <= 0)
                {
                    transform.position -= transform.forward * 0.1f;
                }
            }
        }
    }

    private float distFromPlayer()
    {
        return Vector3.Distance(transform.position, playerCollision.ClosestPoint(transform.position));
    }
}
