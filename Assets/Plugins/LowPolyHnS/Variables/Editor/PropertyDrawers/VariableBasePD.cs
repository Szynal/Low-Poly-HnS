using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(VariableBase), true)]
    public class VariableBasePD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label);
        }
    }
}