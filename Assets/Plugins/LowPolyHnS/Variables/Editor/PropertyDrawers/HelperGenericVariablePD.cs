using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    public abstract class HelperGenericVariablePD : PropertyDrawer
    {
        public const string PROP_ALLOW_TYPES_MASK = "allowTypesMask";
        protected const string PROP_NAME = "name";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spName;
        protected SerializedProperty spAllowTypesMask;

        // PAINT METHODS: -------------------------------------------------------------------------

        protected void PaintVariables(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            spName = property;
            Rect rectLabel = GetRectLabel(position);
            Rect rectField = GetRectField(position);

            EditorGUI.PrefixLabel(rectLabel, label);
            if (EditorGUI.DropdownButton(rectField, new GUIContent(property.stringValue), FocusType.Passive))
            {
                GenericVariableSelectWindow window = GetWindow(rectField);
                if (window != null) PopupWindow.Show(rectField, window);
            }

            EditorGUI.EndProperty();
        }

        // VIRTUAL AND ABSTRACT METHODS: ----------------------------------------------------------

        protected abstract GenericVariableSelectWindow GetWindow(Rect ctaRect);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected void Callback(string name)
        {
            if (spName == null) return;
            spName.stringValue = name;

            spName.serializedObject.ApplyModifiedProperties();
            spName.serializedObject.Update();
        }

        private Rect GetRectLabel(Rect rect)
        {
            return new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
        }

        private Rect GetRectField(Rect rect)
        {
            return new Rect(
                rect.x + EditorGUIUtility.labelWidth,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth,
                rect.height
            );
        }
    }
}