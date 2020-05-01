﻿using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(HelperLocalVariable))]
    public class HelperLocalVariablePD : HelperGenericVariablePD
    {
        private const string PROP_TARGET_TYP = "targetType";
        private const string PROP_TARGET_OBJ = "targetObject";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");
        private const float TARGET_WIDTH = 100f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTargetType;
        private SerializedProperty spTargetObject;

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            spTargetType = property.FindPropertyRelative(PROP_TARGET_TYP);
            spTargetObject = property.FindPropertyRelative(PROP_TARGET_OBJ);
            spAllowTypesMask = property.FindPropertyRelative(PROP_ALLOW_TYPES_MASK);

            Rect rectTargetType = new Rect(
                position.x,
                position.y,
                EditorGUIUtility.labelWidth + TARGET_WIDTH,
                EditorGUI.GetPropertyHeight(spTargetType, true)
            );
            Rect rectTargetObject = new Rect(
                rectTargetType.x + rectTargetType.width + EditorGUIUtility.standardVerticalSpacing,
                rectTargetType.y,
                position.width - (rectTargetType.width + EditorGUIUtility.standardVerticalSpacing),
                EditorGUI.GetPropertyHeight(spTargetObject, true)
            );

            Rect rectVariable = new Rect(
                position.x,
                rectTargetType.y + rectTargetType.height + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rectTargetType, spTargetType, GUICONTENT_EMPTY);
            EditorGUI.BeginDisabledGroup(
                spTargetType.intValue != (int) HelperLocalVariable.Target.GameObject &&
                spTargetType.intValue != (int) HelperLocalVariable.Target.GameObjectPath
            );
            EditorGUI.PropertyField(rectTargetObject, spTargetObject, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            SerializedProperty spName = property.FindPropertyRelative(PROP_NAME);
            PaintLocalVariable(spName, rectVariable, label);

            EditorGUI.EndProperty();
        }

        private void PaintLocalVariable(SerializedProperty spName, Rect rect, GUIContent label)
        {
            if (spTargetType.intValue == (int) HelperLocalVariable.Target.GameObject)
            {
                EditorGUI.BeginDisabledGroup(spTargetObject.objectReferenceValue == null);
                PaintVariables(rect, spName, label);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                string previousName = spName.stringValue;
                EditorGUI.PropertyField(rect, spName, label);
                if (previousName != spName.stringValue)
                {
                    spName.stringValue = VariableEditor.ProcessName(spName.stringValue, true);
                }
            }
        }

        protected override GenericVariableSelectWindow GetWindow(Rect ctaRect)
        {
            if (spTargetObject.objectReferenceValue == null) return null;

            return new LocalVariableSelectWindow(
                ctaRect,
                (GameObject) spTargetObject.objectReferenceValue,
                true,
                Callback,
                spAllowTypesMask == null ? 0 : spAllowTypesMask.intValue
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_NAME), true) +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_TARGET_TYP), true) +
                   EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}