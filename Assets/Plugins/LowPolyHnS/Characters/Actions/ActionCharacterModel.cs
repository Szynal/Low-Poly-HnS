using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterModel : IAction
    {
        public TargetCharacter character = new TargetCharacter();
        public TargetGameObject prefabModel = new TargetGameObject(TargetGameObject.Target.GameObject);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = character.GetCharacter(target);
            GameObject prefab = prefabModel.GetGameObject(target);
            if (charTarget != null && prefab != null)
            {
                CharacterAnimator targetCharAnim = charTarget.GetComponent<CharacterAnimator>();
                if (targetCharAnim.animator != null)
                {
                    targetCharAnim.ChangeModel(prefab);
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Model";
        private const string NODE_TITLE = "Change character {0} model";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spPrefabModel;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, character);
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spPrefabModel = serializedObject.FindProperty("prefabModel");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spPrefabModel = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spPrefabModel);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}