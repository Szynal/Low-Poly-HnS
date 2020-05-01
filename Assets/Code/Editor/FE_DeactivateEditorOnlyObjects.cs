using UnityEditor;
using UnityEngine;

public class FE_DeactivateEditorOnlyObjects
{
    [MenuItem("FearEffect/Deactivate all EditorOnly objects")]
    private static void deactivateObjects()
    {
        GameObject[] _objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < _objects.Length; i++)
        {
            if (_objects[i].CompareTag("EditorOnly") && _objects[i].scene.isLoaded &&
                _objects[i].hideFlags == HideFlags.None)
            {
                _objects[i].SetActive(false);
            }
        }
    }

    [MenuItem("FearEffect/Activate all EditorOnly objects")]
    private static void activateObjects()
    {
        GameObject[] _objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < _objects.Length; i++)
        {
            if (_objects[i].CompareTag("EditorOnly") && _objects[i].scene.isLoaded &&
                _objects[i].hideFlags == HideFlags.None)
            {
                _objects[i].SetActive(true);
            }
        }
    }
}