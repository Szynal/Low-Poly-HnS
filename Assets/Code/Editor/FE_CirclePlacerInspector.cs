using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CirclePlacer))]
public class FE_CirclePlacerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Place objects") == true)
        {
            ((CirclePlacer)target).PlaceObjects();
        }
    }
}
