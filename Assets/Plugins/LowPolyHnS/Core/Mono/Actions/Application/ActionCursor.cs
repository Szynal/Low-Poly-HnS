using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCursor : IAction
    {
        public bool changeTexture = false;
        public Texture2DProperty texture = new Texture2DProperty();
        public Vector2 hotspot = Vector2.zero;
        public bool changeCursorLock = false;
        public CursorLockMode cursorLockMode = CursorLockMode.None;
        public bool changeVisibility = false;
        public bool isVisible = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (changeTexture) Cursor.SetCursor(texture.GetValue(target), hotspot, CursorMode.Auto);
            if (changeCursorLock) Cursor.lockState = cursorLockMode;
            if (changeVisibility) Cursor.visible = isVisible;

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Application/Cursor";
        private const string NODE_TITLE = "Change cursor parameters";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spChangeTexture;
        private SerializedProperty spTexture;
        private SerializedProperty spHotspot;
        private SerializedProperty spChangeCursorLock;
        private SerializedProperty spCursorLockMode;
        private SerializedProperty spChangeVisibility;
        private SerializedProperty spIsVisible;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spChangeTexture = serializedObject.FindProperty("changeTexture");
            spTexture = serializedObject.FindProperty("texture");
            spHotspot = serializedObject.FindProperty("hotspot");
            spChangeCursorLock = serializedObject.FindProperty("changeCursorLock");
            spCursorLockMode = serializedObject.FindProperty("cursorLockMode");
            spChangeVisibility = serializedObject.FindProperty("changeVisibility");
            spIsVisible = serializedObject.FindProperty("isVisible");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spChangeTexture);
            EditorGUI.BeginDisabledGroup(!spChangeTexture.boolValue);
            EditorGUILayout.PropertyField(spTexture);
            EditorGUILayout.PropertyField(spHotspot);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spChangeCursorLock);
            if (spChangeCursorLock.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spCursorLockMode);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spChangeVisibility);
            if (spChangeVisibility.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spIsVisible);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}