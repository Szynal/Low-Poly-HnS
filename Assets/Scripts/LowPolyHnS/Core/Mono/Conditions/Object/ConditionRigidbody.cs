using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionRigidbody : ICondition
    {
        public enum Condition
        {
            IsKinematic,
            IsSleeping
        }

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        public Condition condition = Condition.IsKinematic;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody rb = targetGO.GetComponent<Rigidbody>();
            if (rb == null) return false;

            switch (condition)
            {
                case Condition.IsKinematic: return rb.isKinematic;
                case Condition.IsSleeping: return rb.IsSleeping();
            }

            return false;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Rigidbody";
        private const string NODE_TITLE = "Rigidbody: {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spCondition;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                target,
                condition.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spCondition = serializedObject.FindProperty("condition");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spCondition);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}