using System.IO;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    public class TagsEditorWindow : PopupWindowContent
    {
        private const string TITLE = "Tag Settings";

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private GlobalTagsEditor tagsEditor;
        private Vector2 scroll = Vector2.zero;

        // INITIALIZERS: --------------------------------------------------------------------------

        public override void OnOpen()
        {
            GlobalTags instance = AssetDatabase.LoadAssetAtPath<GlobalTags>(Path.Combine(
                GlobalTagsEditor.PATH_ASSET,
                GlobalTagsEditor.NAME_ASSET
            ));

            if (instance == null) editorWindow.Close();
            tagsEditor = (GlobalTagsEditor) Editor.CreateEditor(instance);
        }

        public override void OnClose()
        {
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 500);
        }

        public override void OnGUI(Rect rect)
        {
            if (tagsEditor == null) return;

            scroll = EditorGUILayout.BeginScrollView(
                scroll,
                EditorStyles.inspectorDefaultMargins
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(TITLE, EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            tagsEditor.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }
    }
}