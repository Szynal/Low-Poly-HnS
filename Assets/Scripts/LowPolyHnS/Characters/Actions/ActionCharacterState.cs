using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterState : IAction
    {
        public enum StateAction
        {
            Change,
            Reset
        }

        public enum StateInput
        {
            StateAsset,
            AnimationClip
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public StateAction action = StateAction.Change;
        public CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        public AvatarMask avatarMask = null;

        public StateInput state = StateInput.StateAsset;
        public CharacterState stateAsset;
        public AnimationClip stateClip;

        [Range(0f, 1f)] public float weight = 1.0f;
        public float transitionTime = 0.25f;
        public NumberProperty speed = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = character.GetCharacter(target);
            if (charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                if (action == StateAction.Reset)
                {
                    charTarget.GetCharacterAnimator().ResetState(transitionTime, layer);
                }
                else if (state == StateInput.StateAsset && stateAsset != null)
                {
                    charTarget.GetCharacterAnimator().SetState(
                        stateAsset,
                        avatarMask,
                        weight,
                        transitionTime,
                        speed.GetValue(target),
                        layer
                    );
                }
                else if (state == StateInput.AnimationClip && stateClip != null)
                {
                    charTarget.GetCharacterAnimator().SetState(
                        stateClip,
                        avatarMask,
                        weight,
                        transitionTime,
                        speed.GetValue(target),
                        layer
                    );
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character State";
        private const string NODE_TITLE = "{0} {1} state in {2}";

        private static readonly GUIContent GC_MASK = new GUIContent("Mask (optional)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spAvatarMask;
        private SerializedProperty spAction;
        private SerializedProperty spLayer;

        private SerializedProperty spState;
        private SerializedProperty spStateAsset;
        private SerializedProperty spStateClip;

        private SerializedProperty spWeight;
        private SerializedProperty spTransitionTime;
        private SerializedProperty spSpeed;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                action == StateAction.Reset ? "Reset" : "Change",
                character,
                layer.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spAvatarMask = serializedObject.FindProperty("avatarMask");
            spAction = serializedObject.FindProperty("action");
            spLayer = serializedObject.FindProperty("layer");

            spState = serializedObject.FindProperty("state");
            spStateAsset = serializedObject.FindProperty("stateAsset");
            spStateClip = serializedObject.FindProperty("stateClip");

            spWeight = serializedObject.FindProperty("weight");
            spTransitionTime = serializedObject.FindProperty("transitionTime");
            spSpeed = serializedObject.FindProperty("speed");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spAvatarMask = null;
            spAction = null;
            spLayer = null;

            spState = null;
            spStateAsset = null;
            spStateClip = null;

            spWeight = null;
            spTransitionTime = null;
            spSpeed = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spAction);
            if (spAction.intValue == (int) StateAction.Change)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spState);
                switch (spState.intValue)
                {
                    case (int) StateInput.StateAsset:
                        EditorGUILayout.PropertyField(spStateAsset);
                        break;

                    case (int) StateInput.AnimationClip:
                        EditorGUILayout.PropertyField(spStateClip);
                        break;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(spAvatarMask, GC_MASK);
                EditorGUILayout.PropertyField(spWeight);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spLayer);
            EditorGUILayout.PropertyField(spTransitionTime);
            EditorGUILayout.PropertyField(spSpeed);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}