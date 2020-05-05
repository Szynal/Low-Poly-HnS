using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLookAt : IAction
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        public TargetPosition lookAtPosition = new TargetPosition();

        [RotationConstraint] public Vector3 freezeRotation = new Vector3(1, 0, 1);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                targetTrans.LookAt(lookAtPosition.GetPosition(target), Vector3.up);

                Vector3 scalar = new Vector3(
                    Mathf.Approximately(freezeRotation.x, 0f) ? 1 : 0,
                    Mathf.Approximately(freezeRotation.y, 0f) ? 1 : 0,
                    Mathf.Approximately(freezeRotation.z, 0f) ? 1 : 0
                );

                targetTrans.localRotation = Quaternion.Euler(Vector3.Scale(
                    targetTrans.localEulerAngles, scalar
                ));
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Look At";
        private const string NODE_TITLE = "{0} look at {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spLookAtPosition;
        private SerializedProperty spConstraints;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                target,
                lookAtPosition
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spLookAtPosition = serializedObject.FindProperty("lookAtPosition");
            spConstraints = serializedObject.FindProperty("freezeRotation");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spLookAtPosition = null;
            spConstraints = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spLookAtPosition);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spConstraints);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}