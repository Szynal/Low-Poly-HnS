using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionMethods : IAction
    {
        public UnityEvent events = new UnityEvent();

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (events != null) events.Invoke();
            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        public static new string NAME = "General/Call Methods";
        private const string NODE_TITLE = "Call {0} method{1}";

        private static readonly GUIContent GUICONTENT_EVENTS = new GUIContent("Call methods");
        private SerializedProperty spEvents;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            int methodsCount = 0;
            string methodsSuffix = "";

            if (events != null)
            {
                methodsCount = events.GetPersistentEventCount();
                methodsSuffix = methodsCount == 1 ? "" : "s";
            }

            return string.Format(NODE_TITLE, methodsCount, methodsSuffix);
        }

        protected override void OnEnableEditorChild()
        {
            spEvents = serializedObject.FindProperty("events");
        }

        protected override void OnDisableEditorChild()
        {
            spEvents = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spEvents, GUICONTENT_EVENTS);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}