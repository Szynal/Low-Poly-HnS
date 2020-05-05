using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTrigger : IAction
    {
        public Trigger trigger;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (trigger != null) trigger.Execute(target);
            return true;
        }

        // +-----------------------------------------------------------------------------------------------------------+
        // | EDITOR                                                                                                    |
        // +-----------------------------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Trigger";
        private const string NODE_TITLE = "Trigger {0}";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spTrigger;

        // INSPECTOR METHODS: ------------------------------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                trigger == null ? "none" : trigger.gameObject.name
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTrigger = serializedObject.FindProperty("trigger");
        }

        protected override void OnDisableEditorChild()
        {
            spTrigger = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(spTrigger);
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}