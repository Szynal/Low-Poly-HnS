using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionGravity : IAction
    {
        public Vector3Property gravity = new Vector3Property(new Vector3(0.0f, -9.81f, 0.0f));

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Physics.gravity = gravity.GetValue(target);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Gravity";
        private const string NODE_TITLE = "Change gravity";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spGravity;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spGravity = serializedObject.FindProperty("gravity");
        }

        protected override void OnDisableEditorChild()
        {
            spGravity = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spGravity);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}