using System.Collections.Generic;
using UnityEngine;

public class CirclePlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToPlace = null;
    [SerializeField] private Transform center = null;
    [SerializeField] private float radius = 1f;

    public void PlaceObjects()
    {
        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            float angle = (float) i / objectsToPlace.Count * Mathf.PI * 2f;
            float posX = Mathf.Sin(angle) * radius;
            float posY = Mathf.Cos(angle) * radius;

            objectsToPlace[i].transform.position = new Vector3(center.position.x + posX, center.position.y + posY,
                objectsToPlace[i].transform.position.z);
            objectsToPlace[i].transform.rotation = Quaternion.Euler(0f, 0f, -(360f / objectsToPlace.Count) * i);
        }
    }
}