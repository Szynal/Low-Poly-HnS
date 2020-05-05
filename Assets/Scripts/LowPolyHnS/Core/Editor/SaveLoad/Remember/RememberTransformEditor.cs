using UnityEditor;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(RememberTransform))]
    public class RememberTransformEditor : RememberEditor
    {
        private SerializedProperty spRememberPosition;
        private SerializedProperty spRememberRotation;
        private SerializedProperty spRememberScale;

        protected override string Comment()
        {
            return "Automatically save and restore Transform properties when loading the game";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            spRememberPosition = serializedObject.FindProperty("rememberPosition");
            spRememberRotation = serializedObject.FindProperty("rememberRotation");
            spRememberScale = serializedObject.FindProperty("rememberScale");
        }

        protected override void OnPaint()
        {
            EditorGUILayout.PropertyField(spRememberPosition);
            EditorGUILayout.PropertyField(spRememberRotation);
            EditorGUILayout.PropertyField(spRememberScale);
        }
    }
}