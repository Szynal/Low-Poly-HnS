using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(HelperGlobalVariable))]
    public class HelperGlobalVariablePD : HelperGenericVariablePD
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            spAllowTypesMask = property.FindPropertyRelative(PROP_ALLOW_TYPES_MASK);

            PaintVariables(
                position,
                property.FindPropertyRelative(PROP_NAME),
                label
            );
        }

        protected override GenericVariableSelectWindow GetWindow(Rect ctaRect)
        {
            return new GlobalVariableSelectWindow(
                ctaRect,
                Callback,
                spAllowTypesMask == null ? 0 : spAllowTypesMask.intValue
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_NAME));
        }
    }
}