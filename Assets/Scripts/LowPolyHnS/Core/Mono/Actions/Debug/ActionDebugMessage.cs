using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionDebugMessage : IAction
    {
        public enum LogType
        {
            NORMAL,
            WARNING,
            ERROR
        }

        public LogType logType = LogType.NORMAL;
        public string message = "debug message";

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (logType)
            {
                case LogType.NORMAL:
                    Debug.Log(message, gameObject);
                    break;
                case LogType.WARNING:
                    Debug.LogWarning(message, gameObject);
                    break;
                case LogType.ERROR:
                    Debug.LogError(message, gameObject);
                    break;
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Message";
        private const string NODE_TITLE = "Debug.{0}: {1}";

        private static readonly GUIContent GUICONTENT_LOGTYPE = new GUIContent("Message Type");
        private static readonly GUIContent GUICONTENT_MESSAGE = new GUIContent("Message");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spLogType;
        private SerializedProperty spMessage;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string type = "Log";
            if (logType == LogType.WARNING) type = "Warning";
            if (logType == LogType.ERROR) type = "Error";

            return string.Format(NODE_TITLE, type, message);
        }

        protected override void OnEnableEditorChild()
        {
            spLogType = serializedObject.FindProperty("logType");
            spMessage = serializedObject.FindProperty("message");
        }

        protected override void OnDisableEditorChild()
        {
            spLogType = null;
            spMessage = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spLogType, GUICONTENT_LOGTYPE);
            EditorGUILayout.PropertyField(spMessage, GUICONTENT_MESSAGE);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}