using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterProperies : IAction
    {
        public enum CHANGE_PROPERTY
        {
            IsControllable,
            CanRun,
            SetRunSpeed,
            Height,
            JumpForce,
            JumpTimes,
            Gravity,
            MaxFallSpeed,
            CanJump
        }

        public TargetCharacter target = new TargetCharacter(TargetCharacter.Target.Player);

        public CHANGE_PROPERTY changeProperty = CHANGE_PROPERTY.IsControllable;

        public BoolProperty valueBool = new BoolProperty(true);
        public NumberProperty valueNumber = new NumberProperty(5.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.target.GetCharacter(target);
            if (charTarget != null)
            {
                switch (changeProperty)
                {
                    case CHANGE_PROPERTY.IsControllable:
                        bool isControllable = valueBool.GetValue(target);
                        charTarget.characterLocomotion.SetIsControllable(isControllable);
                        break;

                    case CHANGE_PROPERTY.CanRun:
                        charTarget.characterLocomotion.canRun = valueBool.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.SetRunSpeed:
                        charTarget.characterLocomotion.runSpeed = valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.Height:
                        charTarget.characterLocomotion.ChangeHeight(valueNumber.GetValue(target));
                        break;

                    case CHANGE_PROPERTY.JumpForce:
                        charTarget.characterLocomotion.jumpForce = valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.JumpTimes:
                        charTarget.characterLocomotion.jumpTimes = valueNumber.GetInt(target);
                        break;

                    case CHANGE_PROPERTY.Gravity:
                        charTarget.characterLocomotion.gravity = valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.MaxFallSpeed:
                        charTarget.characterLocomotion.maxFallSpeed = valueNumber.GetValue(target);
                        break;

                    case CHANGE_PROPERTY.CanJump:
                        charTarget.characterLocomotion.canJump = valueBool.GetValue(target);
                        break;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Change Property";
        private const string NODE_TITLE = "Change {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spChangeProperty;
        private SerializedProperty spValueBool;
        private SerializedProperty spValueNumber;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                target,
                changeProperty
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spChangeProperty = serializedObject.FindProperty("changeProperty");
            spValueBool = serializedObject.FindProperty("valueBool");
            spValueNumber = serializedObject.FindProperty("valueNumber");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spChangeProperty = null;
            spValueBool = null;
            spValueNumber = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spChangeProperty);

            EditorGUILayout.Space();
            switch ((CHANGE_PROPERTY) spChangeProperty.intValue)
            {
                case CHANGE_PROPERTY.IsControllable:
                    EditorGUILayout.PropertyField(spValueBool);
                    break;
                case CHANGE_PROPERTY.CanRun:
                    EditorGUILayout.PropertyField(spValueBool);
                    break;
                case CHANGE_PROPERTY.SetRunSpeed:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.Height:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.JumpForce:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.JumpTimes:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.Gravity:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.MaxFallSpeed:
                    EditorGUILayout.PropertyField(spValueNumber);
                    break;
                case CHANGE_PROPERTY.CanJump:
                    EditorGUILayout.PropertyField(spValueBool);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}