using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionInputKey : ICondition
    {
        public enum STATE
        {
            BeingPressed,
            JustPressed,
            JustReleased
        }

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        public KeyCode key = KeyCode.Space;
        public STATE state = STATE.JustReleased;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            bool result = false;
            switch (state)
            {
                case STATE.BeingPressed:
                    result = Input.GetKey(key);
                    break;
                case STATE.JustPressed:
                    result = Input.GetKeyDown(key);
                    break;
                case STATE.JustReleased:
                    result = Input.GetKeyUp(key);
                    break;
            }

            return result;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Input/Keyboard";
        private const string NODE_TITLE = "Is {0} {1}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spKey;
        private SerializedProperty spState;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string keyName = key.ToString();
            string keyState = "";
            switch (state)
            {
                case STATE.BeingPressed:
                    keyState = "Being Pressed";
                    break;
                case STATE.JustPressed:
                    keyState = "Just Pressed";
                    break;
                case STATE.JustReleased:
                    keyState = "Just Released";
                    break;
            }

            return string.Format(
                NODE_TITLE,
                keyName,
                keyState
            );
        }

        protected override void OnEnableEditorChild()
        {
            spKey = serializedObject.FindProperty("key");
            spState = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spKey);
            EditorGUILayout.PropertyField(spState);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}