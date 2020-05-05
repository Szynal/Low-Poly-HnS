using UnityEditor;

namespace LowPolyHnS.Pool
{
    [CustomEditor(typeof(PoolObject))]
    public class PoolObjectEditor : Editor
    {
        private SerializedProperty spInitCount;
        private SerializedProperty spDuration;

        private void OnEnable()
        {
            spInitCount = serializedObject.FindProperty("initCount");
            spDuration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spInitCount);
            EditorGUILayout.PropertyField(spDuration);

            serializedObject.ApplyModifiedProperties();
        }
    }
}