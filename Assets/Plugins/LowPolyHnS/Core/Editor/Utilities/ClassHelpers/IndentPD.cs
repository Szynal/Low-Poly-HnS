using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentPD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel--;
        }
    }
}