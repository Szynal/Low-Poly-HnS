using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterRagdoll : IAction
    {
        public enum Operation
        {
            Ragdoll,
            Recover
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public Operation turnTo = Operation.Ragdoll;
        public bool autoRecover = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character c = character.GetCharacter(target);
            if (c != null)
            {
                switch (turnTo)
                {
                    case Operation.Ragdoll:
                        c.SetRagdoll(true, autoRecover);
                        break;
                    case Operation.Recover:
                        c.SetRagdoll(false);
                        break;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Ragdoll";
        private const string NODE_TITLE = "{0} character {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spTurnTo;
        private SerializedProperty spAutoRecover;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, turnTo, character);
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spTurnTo = serializedObject.FindProperty("turnTo");
            spAutoRecover = serializedObject.FindProperty("autoRecover");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spTurnTo = null;
            spAutoRecover = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spTurnTo);

            if (spTurnTo.intValue == (int) Operation.Ragdoll)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spAutoRecover);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}