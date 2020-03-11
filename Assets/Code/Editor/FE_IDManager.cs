using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FE_IDManager
{
    [MenuItem("FearEffect/Assign IDs to saveables")]
    static void assignIDs()
    {
        List<int> _usedIDs = new List<int>();

        foreach (MonoBehaviour _mb in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (_mb is ISaveable && _mb.gameObject.scene.isLoaded == true && _mb.gameObject.hideFlags == HideFlags.None)
            {
                SerializedObject _serialized = new SerializedObject(_mb);
                SerializedProperty _idVal = _serialized.FindProperty("SaveableID");

                if (_idVal != null)
                {
                    EditorUtility.SetDirty(_mb);
                    int _randID = -1;
                    do
                    {
                        _randID = Random.Range(0, 9999);
                    } while (_usedIDs.Contains(_randID) == true);

                    _idVal.intValue = _randID;
                    _usedIDs.Add(_randID);
                    _serialized.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("IDManager couldn't assign ID to " + _mb + "  . _idVal was null?");
                }
            }
        }
    }
}
