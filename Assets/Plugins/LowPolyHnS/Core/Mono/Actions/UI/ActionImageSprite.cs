using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionImageSprite : IAction
    {
        public Image image;
        public SpriteProperty sprite = new SpriteProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (image != null)
            {
                image.sprite = sprite.GetValue(target);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "UI/Image Sprite";
        private const string NODE_TITLE = "Change Image sprite";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spImage;
        private SerializedProperty spSprite;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spImage = serializedObject.FindProperty("image");
            spSprite = serializedObject.FindProperty("sprite");
        }

        protected override void OnDisableEditorChild()
        {
            spImage = null;
            spSprite = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spImage);
            EditorGUILayout.PropertyField(spSprite);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}