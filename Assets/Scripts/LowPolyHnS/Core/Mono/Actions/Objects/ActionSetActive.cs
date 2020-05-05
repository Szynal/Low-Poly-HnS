using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionSetActive : IAction
    {
        public TargetGameObject target = new TargetGameObject();
        public bool active = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetValue = this.target.GetGameObject(target);
            if (targetValue != null) targetValue.SetActive(active);

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Set Active";
        private const string NODE_TITLE = "Set {0} object {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spActive;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                active ? "active" : "inactive",
                target
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spActive = serializedObject.FindProperty("active");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spActive = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spActive);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}