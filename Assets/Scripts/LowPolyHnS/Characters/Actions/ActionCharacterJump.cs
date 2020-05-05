using LowPolyHnS.Characters;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterJump : IAction
    {
        public TargetCharacter target = new TargetCharacter();

        public bool overrideJumpForce = false;
        public NumberProperty jumpForce = new NumberProperty(10f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.target.GetCharacter(target);
            if (charTarget != null)
            {
                if (overrideJumpForce) charTarget.Jump(jumpForce.GetValue(target));
                else charTarget.Jump();
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Jump";
        private const string NODE_TITLE = "Jump character {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spOverrideJumpForce;
        private SerializedProperty spJumpForce;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, target);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spOverrideJumpForce = serializedObject.FindProperty("overrideJumpForce");
            spJumpForce = serializedObject.FindProperty("jumpForce");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spOverrideJumpForce = null;
            spJumpForce = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spOverrideJumpForce);

            EditorGUI.BeginDisabledGroup(!spOverrideJumpForce.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spJumpForce);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}