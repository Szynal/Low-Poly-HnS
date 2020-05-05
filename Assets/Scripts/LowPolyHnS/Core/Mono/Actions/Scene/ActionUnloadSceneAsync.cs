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
    public class ActionUnloadSceneAsync : IAction
    {
        public StringProperty sceneName = new StringProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            string scene = sceneName.GetValue(target);
            AsyncOperation async = SceneManager.UnloadSceneAsync(scene);

            yield return async;
            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Scene/Unload Scene Async";
        private const string NODE_TITLE = "Unload scene {0} async";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSceneName;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, sceneName);
        }

        protected override void OnEnableEditorChild()
        {
            spSceneName = serializedObject.FindProperty("sceneName");
        }

        protected override void OnDisableEditorChild()
        {
            spSceneName = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spSceneName);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}