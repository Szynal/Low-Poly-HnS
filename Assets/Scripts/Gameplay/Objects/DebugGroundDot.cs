using UnityEngine;

public class DebugGroundDot : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z); ;
            }
        }
    }
}