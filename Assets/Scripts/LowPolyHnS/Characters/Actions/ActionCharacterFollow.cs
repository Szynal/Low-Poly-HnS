using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterFollow : IAction
    {
        public enum ActionType
        {
            Follow,
            StopFollow
        }

        public TargetCharacter character = new TargetCharacter();
        public ActionType actionType = ActionType.Follow;

        public TargetGameObject followTarget = new TargetGameObject();
        public float followMinRadius = 2.0f;
        public float followMaxRadius = 4.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = character.GetCharacter(target);
            if (charTarget == null) return true;

            Transform follow = null;

            if (actionType == ActionType.Follow)
            {
                follow = followTarget.GetComponent<Transform>(target);
            }

            charTarget.characterLocomotion.FollowTarget(
                follow,
                followMinRadius,
                followMaxRadius
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Follow";
        private const string NODE_TITLE = "{0} {1} {2}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spActionType;

        private SerializedProperty spFollowTarget;
        private SerializedProperty spMinRadius;
        private SerializedProperty spMaxRadius;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                character,
                ObjectNames.NicifyVariableName(actionType.ToString()),
                actionType == ActionType.Follow ? followTarget.ToString() : ""
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spActionType = serializedObject.FindProperty("actionType");
            spFollowTarget = serializedObject.FindProperty("followTarget");
            spMinRadius = serializedObject.FindProperty("followMinRadius");
            spMaxRadius = serializedObject.FindProperty("followMaxRadius");
        }

        protected override void OnDisableEditorChild()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spActionType);

            if ((ActionType) spActionType.intValue == ActionType.Follow)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(spFollowTarget);

                EditorGUILayout.PropertyField(spMinRadius);
                EditorGUILayout.PropertyField(spMaxRadius);
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}