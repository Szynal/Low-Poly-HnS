using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterHand : IAction
    {
        public enum Action
        {
            Reach,
            LetGo
        }

        public TargetCharacter character = new TargetCharacter();
        public Action action = Action.Reach;

        public CharacterHandIK.Limb hand = CharacterHandIK.Limb.LeftHand;
        public TargetGameObject reachTarget = new TargetGameObject();

        [Range(0.01f, 5.0f)] public float duration = 0.5f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character characterTarget = character.GetCharacter(target);
            if (characterTarget == null) return true;

            CharacterAnimator animator = characterTarget.GetCharacterAnimator();
            if (animator == null) return true;

            CharacterHandIK handIK = animator.GetCharacterHandIK();
            if (handIK == null) return true;

            switch (action)
            {
                case Action.Reach:
                    handIK.Reach(
                        hand,
                        reachTarget.GetTransform(target),
                        duration
                    );
                    break;

                case Action.LetGo:
                    handIK.LetGo(hand, duration);
                    break;
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Hand";
        private const string NODE_TITLE = "{0} with {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spAction;

        private SerializedProperty spHand;
        private SerializedProperty spReachTarget;
        private SerializedProperty spDuration;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                action.ToString(),
                hand.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spAction = serializedObject.FindProperty("action");

            spHand = serializedObject.FindProperty("hand");
            spReachTarget = serializedObject.FindProperty("reachTarget");
            spDuration = serializedObject.FindProperty("duration");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spAction = null;

            spHand = null;
            spReachTarget = null;
            spDuration = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spAction);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spHand);
            if (spAction.intValue == (int) Action.Reach)
            {
                EditorGUILayout.PropertyField(spReachTarget);
            }

            EditorGUILayout.PropertyField(spDuration);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}