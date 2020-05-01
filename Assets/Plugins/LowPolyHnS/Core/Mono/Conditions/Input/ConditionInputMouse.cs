using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionInputMouse : ICondition
    {
        public enum MOUSE_BUTTON
        {
            LeftButton = 0,
            RightButton = 1,
            MiddleButton = 2
        }

        public enum STATE
        {
            BeingPressed,
            JustPressed,
            JustReleased
        }

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        public MOUSE_BUTTON button = MOUSE_BUTTON.LeftButton;
        public STATE state = STATE.BeingPressed;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool Check()
        {
            bool result = false;

            switch (state)
            {
                case STATE.BeingPressed:
                    result = Input.GetMouseButton((int) button);
                    break;
                case STATE.JustPressed:
                    result = Input.GetMouseButtonDown((int) button);
                    break;
                case STATE.JustReleased:
                    result = Input.GetMouseButtonUp((int) button);
                    break;
            }

            return result;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Input/Mouse Button";
        private const string NODE_TITLE = "Is Mouse {0} {1}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spButton;
        private SerializedProperty spState;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string buttonName = "";
            string buttonState = "";

            switch (button)
            {
                case MOUSE_BUTTON.LeftButton:
                    buttonName = "Left Button";
                    break;
                case MOUSE_BUTTON.RightButton:
                    buttonName = "Right Button";
                    break;
                case MOUSE_BUTTON.MiddleButton:
                    buttonName = "Middle Button";
                    break;
            }

            switch (state)
            {
                case STATE.BeingPressed:
                    buttonState = "Being Pressed";
                    break;
                case STATE.JustPressed:
                    buttonState = "Just Pressed";
                    break;
                case STATE.JustReleased:
                    buttonState = "Just Released";
                    break;
            }

            return string.Format(
                NODE_TITLE,
                buttonName,
                buttonState
            );
        }

        protected override void OnEnableEditorChild()
        {
            spButton = serializedObject.FindProperty("button");
            spState = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spButton);
            EditorGUILayout.PropertyField(spState);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}