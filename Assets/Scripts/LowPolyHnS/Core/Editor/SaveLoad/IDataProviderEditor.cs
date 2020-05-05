using UnityEditor;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(IDataProvider), true)]
    public class IDataProviderEditor : Editor
    {
        private SerializedProperty spTitle;
        private SerializedProperty spDescription;

        private void OnEnable()
        {
            spTitle = serializedObject.FindProperty("title");
            spDescription = serializedObject.FindProperty("description");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(spTitle.stringValue, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(spDescription.stringValue, MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}