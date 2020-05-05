using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public class LayerSelectorPD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}