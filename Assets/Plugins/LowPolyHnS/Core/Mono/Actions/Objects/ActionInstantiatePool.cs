using LowPolyHnS.Pool;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionInstantiatePool : IAction
    {
        public TargetGameObject prefab = new TargetGameObject();
        public TargetPosition initLocation = new TargetPosition();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject prefabValue = prefab.GetGameObject(target);
            if (prefabValue != null)
            {
                Vector3 position = initLocation.GetPosition(target, Space.Self);
                Quaternion rotation = initLocation.GetRotation(target);

                GameObject instance = PoolManager.Instance.Pick(prefabValue);
                instance.transform.SetPositionAndRotation(position, rotation);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Instantiate from Pool";
        private const string NODE_TITLE = "Instantiate {0} from Pool";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPrefab;
        private SerializedProperty spInitLocation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, prefab);
        }

        protected override void OnEnableEditorChild()
        {
            spPrefab = serializedObject.FindProperty("prefab");
            spInitLocation = serializedObject.FindProperty("initLocation");
        }

        protected override void OnDisableEditorChild()
        {
            spPrefab = null;
            spInitLocation = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spPrefab);
            EditorGUILayout.PropertyField(spInitLocation);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}