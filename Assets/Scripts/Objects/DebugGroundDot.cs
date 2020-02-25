﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGroundDot : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                transform.position = hit.point;
            }
        }
    }
}
