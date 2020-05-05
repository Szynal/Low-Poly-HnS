using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterDirection : IAction
    {
        public TargetCharacter character = new TargetCharacter();

        public CharacterLocomotion.FACE_DIRECTION direction;
        public TargetPosition directionTarget = new TargetPosition(TargetPosition.Target.Transform);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character c = character.GetCharacter(target);

            if (c != null)
            {
                c.characterLocomotion.faceDirection = direction;
                if (direction == CharacterLocomotion.FACE_DIRECTION.Target)
                {
                    TargetPosition dirTarget = directionTarget;
                    if (dirTarget.target == TargetPosition.Target.Invoker)
                    {
                        dirTarget.target = TargetPosition.Target.Transform;
                        dirTarget.targetTransform = target.transform;
                    }

                    c.characterLocomotion.faceDirectionTarget = dirTarget;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Direction";
        private const string NODE_TITLE = "Change {0} direction to {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spDirection;
        private SerializedProperty spDirectionTarget;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                character,
                direction.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spDirection = serializedObject.FindProperty("direction");
            spDirectionTarget = serializedObject.FindProperty("directionTarget");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spDirection = null;
            spDirectionTarget = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDirection);
            if (spDirection.intValue == (int) CharacterLocomotion.FACE_DIRECTION.Target)
            {
                EditorGUILayout.PropertyField(spDirectionTarget);
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}