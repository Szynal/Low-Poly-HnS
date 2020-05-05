using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Localization
{
    [CustomEditor(typeof(DatabaseGeneral))]
    public class DatabaseGeneralEditor : IDatabaseEditor
    {
        private const string MSG_DP = "The default PlayerPrefs will be used if no Data Provider is selected";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spGeneralRenderMode;
        private SerializedProperty spPrefabFloatingMessage;
        private SerializedProperty spPrefabSimpleMessage;
        private SerializedProperty spPrefabTouchstick;
        private SerializedProperty spSaveScenes;
        private SerializedProperty spProvider;
        private SerializedProperty spToolbarPositionX;
        private SerializedProperty spToolbarPositionY;

        private SerializedProperty spDefaultMusicAudioMixer;
        private SerializedProperty spDefaultSoundAudioMixer;
        private SerializedProperty spDefaultVoiceAudioMixer;

        private Editor editorDataProvider;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            spGeneralRenderMode = serializedObject.FindProperty("generalRenderMode");
            spPrefabFloatingMessage = serializedObject.FindProperty("prefabFloatingMessage");
            spPrefabSimpleMessage = serializedObject.FindProperty("prefabSimpleMessage");
            spPrefabTouchstick = serializedObject.FindProperty("prefabTouchstick");
            spSaveScenes = serializedObject.FindProperty("saveScenes");
            spProvider = serializedObject.FindProperty("provider");
            spToolbarPositionX = serializedObject.FindProperty("toolbarPositionX");
            spToolbarPositionY = serializedObject.FindProperty("toolbarPositionY");

            spDefaultMusicAudioMixer = serializedObject.FindProperty("musicAudioMixer");
            spDefaultSoundAudioMixer = serializedObject.FindProperty("soundAudioMixer");
            spDefaultVoiceAudioMixer = serializedObject.FindProperty("voiceAudioMixer");

            InitEditorDataProvider();
        }

        private void InitEditorDataProvider()
        {
            Object dataProvider = spProvider.objectReferenceValue;
            if (dataProvider == null)
            {
                editorDataProvider = null;
                return;
            }

            editorDataProvider = CreateEditor(dataProvider);
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string GetName()
        {
            return "General";
        }

        public override int GetPanelWeight()
        {
            return 98;
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spGeneralRenderMode);
            EditorGUILayout.PropertyField(spPrefabFloatingMessage);
            EditorGUILayout.PropertyField(spPrefabSimpleMessage);
            EditorGUILayout.PropertyField(spPrefabTouchstick);

            PaintProvider();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spDefaultMusicAudioMixer);
            EditorGUILayout.PropertyField(spDefaultSoundAudioMixer);
            EditorGUILayout.PropertyField(spDefaultVoiceAudioMixer);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Toolbar", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spToolbarPositionX);
            EditorGUILayout.PropertyField(spToolbarPositionY);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintProvider()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save/Load System:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spSaveScenes);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spProvider, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) InitEditorDataProvider();

            if (editorDataProvider != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                editorDataProvider.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox(MSG_DP, MessageType.Info);
            }
        }
    }
}