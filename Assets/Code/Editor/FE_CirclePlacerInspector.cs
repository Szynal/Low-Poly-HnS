using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FE_CirclePlacer))]
public class FE_CirclePlacerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Place objects") == true)
        {
            ((FE_CirclePlacer)target).PlaceObjects();
        }
    }
}
