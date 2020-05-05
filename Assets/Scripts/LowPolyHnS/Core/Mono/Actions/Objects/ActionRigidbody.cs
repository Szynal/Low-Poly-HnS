using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionRigidbody : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);

        public bool changeMass = false;
        public bool changeUseGravity = false;
        public bool changeIsKinematic = false;

        public NumberProperty mass = new NumberProperty(1f);
        public BoolProperty useGravity = new BoolProperty(true);
        public BoolProperty isKinematic = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody targetRB = targetGO.GetComponent<Rigidbody>();
            if (targetRB != null)
            {
                targetRB.mass = mass.GetValue(target);
                targetRB.useGravity = useGravity.GetValue(target);
                targetRB.isKinematic = isKinematic.GetValue(target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Rigidbody";
        private const string NODE_TITLE = "Change {0} properties";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spTarget;
        public SerializedProperty spChangeMass;
        public SerializedProperty spChangeUseGravity;
        public SerializedProperty spChangeIsKinematic;
        public SerializedProperty spMass;
        public SerializedProperty spUseGravity;
        public SerializedProperty spIsKinematic;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, target);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spChangeMass = serializedObject.FindProperty("changeMass");
            spChangeUseGravity = serializedObject.FindProperty("changeUseGravity");
            spChangeIsKinematic = serializedObject.FindProperty("changeIsKinematic");

            spMass = serializedObject.FindProperty("mass");
            spUseGravity = serializedObject.FindProperty("useGravity");
            spIsKinematic = serializedObject.FindProperty("isKinematic");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spChangeMass = null;
            spChangeUseGravity = null;
            spChangeIsKinematic = null;
            spMass = null;
            spUseGravity = null;
            spIsKinematic = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.Space();

            PaintSection(spChangeMass, spMass);
            EditorGUILayout.Space();
            PaintSection(spChangeUseGravity, spUseGravity);
            EditorGUILayout.Space();
            PaintSection(spChangeIsKinematic, spIsKinematic);

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintSection(SerializedProperty spChange, SerializedProperty spProperty)
        {
            EditorGUILayout.PropertyField(spChange, new GUIContent(spProperty.displayName));
            EditorGUI.BeginDisabledGroup(!spChange.boolValue);
            EditorGUILayout.PropertyField(spProperty, GUICONTENT_EMPTY);
            EditorGUI.EndDisabledGroup();
        }

#endif
    }
}