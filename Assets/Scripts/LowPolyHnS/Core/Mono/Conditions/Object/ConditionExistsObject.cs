using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionExistsObject : ICondition
    {
        public TargetGameObject target = new TargetGameObject();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            return this.target.GetGameObject(target) != null;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Exists GameObject";
        private const string NODE_TITLE = "Does {0} exist";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, target);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}