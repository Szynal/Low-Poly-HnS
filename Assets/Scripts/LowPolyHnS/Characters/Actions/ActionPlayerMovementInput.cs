using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionPlayerMovementInput : IAction
    {
        public PlayerCharacter.INPUT_TYPE inputType = PlayerCharacter.INPUT_TYPE.Wsad;

        public PlayerCharacter.MOUSE_BUTTON mouseButton = PlayerCharacter.MOUSE_BUTTON.LeftClick;
        public bool invertAxis = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (HookPlayer.Instance != null)
            {
                PlayerCharacter player = HookPlayer.Instance.Get<PlayerCharacter>();
                player.inputType = inputType;

                if (inputType == PlayerCharacter.INPUT_TYPE.PointAndClick ||
                    inputType == PlayerCharacter.INPUT_TYPE.FollowPointer)
                {
                    player.mouseButtonMove = mouseButton;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Player Movement Input";
        private const string NODE_TITLE = "Change input to {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spInputType;
        private SerializedProperty spMouseButton;
        private SerializedProperty spInvertAxis;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string value = spInputType.enumDisplayNames[spInputType.intValue];
            if (spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.PointAndClick)
            {
                value = string.Format(
                    "{0} ({1})",
                    value,
                    spMouseButton.enumDisplayNames[spMouseButton.intValue]
                );
            }

            return string.Format(NODE_TITLE, value);
        }

        protected override void OnEnableEditorChild()
        {
            spInputType = serializedObject.FindProperty("inputType");
            spMouseButton = serializedObject.FindProperty("mouseButton");
            spInvertAxis = serializedObject.FindProperty("invertAxis");
        }

        protected override void OnDisableEditorChild()
        {
            spInputType = null;
            spMouseButton = null;
            spInvertAxis = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spInputType);
            if (spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.PointAndClick ||
                spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.FollowPointer)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spMouseButton);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}