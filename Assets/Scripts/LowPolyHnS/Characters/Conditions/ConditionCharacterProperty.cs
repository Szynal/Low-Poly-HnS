using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ConditionCharacterProperty : ICondition
    {
        public enum CharacterProperty
        {
            IsControllable,
            IsIdle,
            IsWalking,
            IsRunning,
            IsGrounded,
            IsOnAir,
            CanRun
        }

        public TargetCharacter target;
        public CharacterProperty property = CharacterProperty.IsIdle;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            Character character = this.target.GetCharacter(target);
            if (character == null) return true;
            bool result = true;

            switch (property)
            {
                case CharacterProperty.IsControllable:
                    result = character.IsControllable();
                    break;

                case CharacterProperty.IsIdle:
                    result = character.GetCharacterMotion() == 0;
                    break;

                case CharacterProperty.IsWalking:
                    result = character.GetCharacterMotion() == 1;
                    break;

                case CharacterProperty.IsRunning:
                    result = character.GetCharacterMotion() == 2;
                    break;

                case CharacterProperty.IsGrounded:
                    result = character.IsGrounded();
                    break;

                case CharacterProperty.IsOnAir:
                    result = !character.IsGrounded();
                    break;

                case CharacterProperty.CanRun:
                    if (character.characterLocomotion != null)
                        result = character.characterLocomotion.canRun;
                    break;
            }

            return result;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        //public const string CUSTOM_ICON_PATH = "Assets/[Custom Path To Icon]";

        public static new string NAME = "Characters/Character Property";
        private const string NODE_TITLE = "Character {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spProperty;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                target == null ? "(undefined)" : target.ToString(),
                property.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spProperty = serializedObject.FindProperty("property");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spProperty);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}