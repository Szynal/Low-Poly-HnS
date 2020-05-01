using System.Collections.Generic;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

#endif

    [AddComponentMenu("")]
    public class ConditionPattern : ICondition
    {
        public enum Value
        {
            True,
            False
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public List<Value> pattern = new List<Value>
        {
            Value.True,
            Value.False
        };

        private int patternIndex;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check()
        {
            if (pattern.Count == 0) return false;
            bool result = pattern[patternIndex] == Value.True;

            patternIndex = ++patternIndex >= pattern.Count ? 0 : patternIndex;
            return result;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Pattern";
        private const string NODE_TITLE = "Follow Pattern";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPattern;
        private ReorderableList list;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spPattern = serializedObject.FindProperty("pattern");
            list = new ReorderableList(
                serializedObject, spPattern,
                true, true, true, true
            );

            list.drawHeaderCallback += PaintHeader;
            list.drawElementCallback += PaintElement;
        }

        private void PaintHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Pattern");
        }

        private void PaintElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = spPattern.GetArrayElementAtIndex(index);
            Rect rectElement = new Rect(
                rect.x,
                rect.y + 1f,
                rect.width,
                rect.height - 2f
            );

            EditorGUI.PropertyField(
                rectElement,
                property,
                new GUIContent(index.ToString())
            );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}