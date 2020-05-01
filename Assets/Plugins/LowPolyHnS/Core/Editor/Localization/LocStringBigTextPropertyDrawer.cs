using System;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Localization
{
    [CustomPropertyDrawer(typeof(LocStringBigTextAttribute))]
    public class LocStringBigTextPropertyDrawer : PropertyDrawer
    {
        private const float TRANSLATION_BUTTON_WIDTH = 25f;
        private const float HORIZONTAL_SEPARATION = -1f;

        private static readonly GUIContent GUICONTENT_TRANSLATION = new GUIContent(
            "Enable Translation",
            "Enable to add a translation slot"
        );

        private SerializedProperty spDatabasePlaceholder;
        private GUIStyle textAreaStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            SerializedProperty spContent = property.FindPropertyRelative("content");
            SerializedProperty spPostProcess = property.FindPropertyRelative("postProcess");
            SerializedProperty spTranslationID = property.FindPropertyRelative("translationID");

            Rect spTranslationIDRect = new Rect(
                position.x, position.y,
                position.width, EditorGUIUtility.singleLineHeight
            );

            bool translationOn = spTranslationID.intValue != 0;
            bool nextTranslationOn = EditorGUI.Toggle(spTranslationIDRect, GUICONTENT_TRANSLATION, translationOn);
            if (PaintPostProcess()) EditorGUILayout.PropertyField(spPostProcess);

            if (textAreaStyle == null)
            {
                textAreaStyle = new GUIStyle();
                textAreaStyle.margin = new RectOffset(5, 5, 0, 0);
                textAreaStyle.wordWrap = true;
            }

            Rect textAreaRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight * 3f,
                EditorGUIUtility.singleLineHeight * 3f,
                textAreaStyle
            );

            spContent.stringValue = EditorGUI.TextArea(
                textAreaRect,
                spContent.stringValue
            );

            if (translationOn != nextTranslationOn)
            {
                if (nextTranslationOn)
                {
                    spTranslationID.intValue = LocStringPropertyDrawer.GenerateID();

                    Editor editorLocalization = Editor.CreateEditor(DatabaseLocalization.Load());
                    ((DatabaseLocalizationEditor) editorLocalization).AddTranslation(spTranslationID.intValue,
                        spContent.stringValue);
                }
                else
                {
                    if (spTranslationID.intValue != 0)
                    {
                        Editor editorLocalization = Editor.CreateEditor(DatabaseLocalization.Load());
                        ((DatabaseLocalizationEditor) editorLocalization).RemoveTranslation(spTranslationID.intValue);
                    }

                    spTranslationID.intValue = 0;
                }
            }

            if (spTranslationID.intValue != 0)
            {
                if (spDatabasePlaceholder == null)
                {
                    Editor editorLocalization = Editor.CreateEditor(DatabaseLocalization.Load());
                    spDatabasePlaceholder = ((DatabaseLocalizationEditor) editorLocalization).GetPlaceholder(
                        spTranslationID.intValue
                    );
                }

                if (spDatabasePlaceholder != null)
                {
                    spDatabasePlaceholder.stringValue = spContent.stringValue;
                    spDatabasePlaceholder.serializedObject.ApplyModifiedProperties();
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        protected virtual bool PaintPostProcess()
        {
            return true;
        }

        // STATIC METHODS: ---------------------------------------------------------------------------------------------

        public static string GenerateID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}