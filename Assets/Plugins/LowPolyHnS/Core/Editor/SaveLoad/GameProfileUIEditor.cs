using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(GameProfileUI), true)]
    [CanEditMultipleObjects]
    public class GameProfileUIEditor : Editor
    {
        private SerializedProperty spProfile;
        private SerializedProperty spTextProfile;
        private SerializedProperty spFormatProfile;
        private SerializedProperty spTextDate;
        private SerializedProperty spFormatDate;

        private static readonly GUIContent GC_FORMAT = new GUIContent("Format");

        private void OnEnable()
        {
            spProfile = serializedObject.FindProperty("profile");
            spTextProfile = serializedObject.FindProperty("textProfile");
            spFormatProfile = serializedObject.FindProperty("formatProfile");
            spTextDate = serializedObject.FindProperty("textDate");
            spFormatDate = serializedObject.FindProperty("formatDate");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spProfile);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spTextProfile);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spFormatProfile, GC_FORMAT);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spTextDate);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spFormatDate, GC_FORMAT);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        // STATIC METHODS: ------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/UI/Game Profile", false, 100)]
        public static void CreateGameProfileUI()
        {
            GameObject text = CreateSceneObject.Create("Profile");
            text.AddComponent<GameProfileUI>();

            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;
        }
    }
}