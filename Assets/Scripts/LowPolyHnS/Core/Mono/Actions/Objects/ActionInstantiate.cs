using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInstantiate : IAction
    {
        public TargetGameObject prefab = new TargetGameObject();
        public TargetPosition initLocation = new TargetPosition();

        [Space] public VariableProperty storeInstance = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject prefabValue = prefab.GetGameObject(target);
            if (prefabValue != null)
            {
                Vector3 position = initLocation.GetPosition(target, Space.Self);
                Quaternion rotation = initLocation.GetRotation(target);

                GameObject instance = Instantiate(prefabValue, position, rotation);
                if (instance != null) storeInstance.Set(instance, target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Instantiate";
        private const string NODE_TITLE = "Instantiate {0}";

        private static readonly GUIContent GC_STORE = new GUIContent("Store (optional)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPrefab;
        private SerializedProperty spInitLocation;
        private SerializedProperty spStore;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, prefab);
        }

        protected override void OnEnableEditorChild()
        {
            spPrefab = serializedObject.FindProperty("prefab");
            spInitLocation = serializedObject.FindProperty("initLocation");
            spStore = serializedObject.FindProperty("storeInstance");
        }

        protected override void OnDisableEditorChild()
        {
            spPrefab = null;
            spInitLocation = null;
            spStore = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spPrefab);
            EditorGUILayout.PropertyField(spInitLocation);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spStore, GC_STORE);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}