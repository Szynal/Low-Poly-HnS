using LowPolyHnS.Characters;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLoadScenePlayer : IAction
    {
        public StringProperty sceneName = new StringProperty();

        public Vector3 playerPosition = Vector3.zero;
        public Quaternion playerRotation = Quaternion.identity;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            PlayerCharacter.ON_LOAD_SCENE_DATA = new Character.OnLoadSceneData(
                playerPosition,
                playerRotation
            );

            SceneManager.LoadScene(sceneName.GetValue(target), LoadSceneMode.Single);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Scene/Load Scene with Player";
        private const string NODE_TITLE = "Load scene {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSceneName;
        private SerializedProperty spPosition;
        private SerializedProperty spRotation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, sceneName);
        }

        protected override void OnEnableEditorChild()
        {
            spSceneName = serializedObject.FindProperty("sceneName");
            spPosition = serializedObject.FindProperty("playerPosition");
            spRotation = serializedObject.FindProperty("playerRotation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSceneName);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spPosition);

            Vector3 rotation = EditorGUILayout.Vector3Field(
                spRotation.displayName,
                spRotation.quaternionValue.eulerAngles
            );

            spRotation.quaternionValue = Quaternion.Euler(rotation);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}