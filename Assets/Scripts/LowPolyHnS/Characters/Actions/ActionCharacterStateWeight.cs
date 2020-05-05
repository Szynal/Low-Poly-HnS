using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterStateWeight : IAction
    {
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        public NumberProperty weight = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = character.GetCharacter(target);
            if (charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                charTarget.GetCharacterAnimator().ChangeStateWeight(
                    layer,
                    weight.GetValue(target)
                );
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character State Weight";
        private const string NODE_TITLE = "Change {0} {1} state weight";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spLayer;
        private SerializedProperty spWeight;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                character,
                layer.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spLayer = serializedObject.FindProperty("layer");
            spWeight = serializedObject.FindProperty("weight");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spLayer = null;
            spWeight = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spLayer);
            EditorGUILayout.PropertyField(spWeight);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}