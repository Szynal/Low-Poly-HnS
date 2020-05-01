using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLoadScene : IAction
    {
        public StringProperty sceneName = new StringProperty();
        public LoadSceneMode mode = LoadSceneMode.Single;
        public bool async = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (async)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(
                    sceneName.GetValue(target),
                    mode
                );

                yield return asyncOperation;
            }
            else
            {
                SceneManager.LoadScene(sceneName.GetValue(target), mode);
                yield return null;
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Scene/Load Scene";
        private const string NODE_TITLE = "Load scene {0}{1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSceneName;
        private SerializedProperty spMode;
        private SerializedProperty spAsync;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                sceneName,
                async ? " (Async)" : ""
            );
        }

        protected override void OnEnableEditorChild()
        {
            spSceneName = serializedObject.FindProperty("sceneName");
            spMode = serializedObject.FindProperty("mode");
            spAsync = serializedObject.FindProperty("async");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSceneName);
            EditorGUILayout.PropertyField(spMode);
            EditorGUILayout.PropertyField(spAsync);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}