using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterAttachment : IAction
    {
        public enum Action
        {
            Attach,
            Detach,
            Remove
        }

        public TargetCharacter character = new TargetCharacter();
        public Action action = Action.Attach;

        public HumanBodyBones bone = HumanBodyBones.RightHand;
        public TargetGameObject instance = new TargetGameObject();
        public Space space = Space.Self;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character characterTarget = character.GetCharacter(target);
            if (characterTarget == null) return true;

            CharacterAnimator animator = characterTarget.GetCharacterAnimator();
            if (animator == null) return true;

            CharacterAttachments attachments = animator.GetCharacterAttachments();
            if (attachments == null) return true;

            switch (action)
            {
                case Action.Attach:
                    attachments.Attach(
                        bone,
                        instance.GetGameObject(target),
                        position,
                        Quaternion.Euler(rotation),
                        space
                    );
                    break;

                case Action.Detach:
                    attachments.Detach(bone);
                    break;

                case Action.Remove:
                    attachments.Remove(bone);
                    break;
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Attachment";
        private const string NODE_TITLE = "{0} from {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spAction;

        private SerializedProperty spBone;
        private SerializedProperty spInstance;
        private SerializedProperty spSpace;
        private SerializedProperty spPosition;
        private SerializedProperty spRotation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                action.ToString(),
                bone.ToString()
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spAction = serializedObject.FindProperty("action");

            spBone = serializedObject.FindProperty("bone");
            spInstance = serializedObject.FindProperty("instance");
            spSpace = serializedObject.FindProperty("space");
            spPosition = serializedObject.FindProperty("position");
            spRotation = serializedObject.FindProperty("rotation");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spAction = null;

            spBone = null;
            spInstance = null;
            spSpace = null;
            spPosition = null;
            spRotation = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spAction);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spBone);
            if (spAction.intValue == (int) Action.Attach)
            {
                EditorGUILayout.PropertyField(spInstance);
                EditorGUILayout.PropertyField(spSpace);
                EditorGUILayout.PropertyField(spPosition);
                EditorGUILayout.PropertyField(spRotation);
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}