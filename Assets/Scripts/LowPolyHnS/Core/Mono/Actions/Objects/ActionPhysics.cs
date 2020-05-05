using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionPhysics : IAction
    {
        public Vector3 force = Vector3.zero;
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);

        public ForceMode forceMode = ForceMode.Impulse;
        public Space spaceMode = Space.Self;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody targetRB = targetGO.GetComponent<Rigidbody>();
            if (targetRB != null)
            {
                Vector3 directionForce = Vector3.zero;

                if (spaceMode == Space.World) directionForce = force;
                else directionForce = targetRB.transform.TransformDirection(force);

                targetRB.AddForce(directionForce, forceMode);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Physics";
        private const string NODE_TITLE = "Add {0} ({1:0.0},{2:0.0},{3:0.0}) to {4}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spForce;
        private SerializedProperty spTarget;
        private SerializedProperty spForceMode;
        private SerializedProperty spSpaceMode;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                forceMode.ToString(),
                force.x,
                force.y,
                force.z,
                target
            );
        }

        protected override void OnEnableEditorChild()
        {
            spForce = serializedObject.FindProperty("force");
            spTarget = serializedObject.FindProperty("target");
            spForceMode = serializedObject.FindProperty("forceMode");
            spSpaceMode = serializedObject.FindProperty("spaceMode");
        }

        protected override void OnDisableEditorChild()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spForce);
            EditorGUILayout.PropertyField(spForceMode);
            EditorGUILayout.PropertyField(spSpaceMode);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}