using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_CirclePlacer : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToPlace = null;
    [SerializeField] Transform center = null;
    [SerializeField] float radius = 1f;

    public void PlaceObjects()
    {

        for(int i = 0; i < objectsToPlace.Count; i++)
        {

            float _angle = ((float)i / objectsToPlace.Count) * Mathf.PI * 2f;       

            float _posX = Mathf.Sin(_angle) * radius;
            float _posY = Mathf.Cos(_angle) * radius;

            objectsToPlace[i].transform.position = new Vector3(center.position.x + _posX, center.position.y + _posY, objectsToPlace[i].transform.position.z);
            objectsToPlace[i].transform.rotation = Quaternion.Euler(0f, 0f, -(360f / objectsToPlace.Count) * i);
        }
    }
}
