using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionGraphicColor : IAction
    {
        public Graphic graphic;

        [Range(0.0f, 10.0f)] public float duration = 0.0f;

        public ColorProperty color = new ColorProperty(Color.white);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (duration <= 0.0f)
            {
                if (graphic != null) graphic.color = color.GetValue(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (graphic != null)
            {
                Color currentColor = graphic.color;
                Color targetColor = color.GetValue(target);

                float startTime = Time.unscaledTime;
                WaitUntil waitUntil = new WaitUntil(() =>
                {
                    float t = (Time.unscaledTime - startTime) / duration;
                    graphic.color = Color.Lerp(currentColor, targetColor, t);

                    return t > 1.0f;
                });

                yield return waitUntil;
                graphic.color = targetColor;
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "UI/Graphic Color";
        private const string NODE_TITLE = "Change Graphic Color";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spGraphic;
        private SerializedProperty spDuration;
        private SerializedProperty spColor;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spGraphic = serializedObject.FindProperty("graphic");
            spDuration = serializedObject.FindProperty("duration");
            spColor = serializedObject.FindProperty("color");
        }

        protected override void OnDisableEditorChild()
        {
            spGraphic = null;
            spDuration = null;
            spColor = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spGraphic);
            EditorGUILayout.PropertyField(spDuration);
            EditorGUILayout.PropertyField(spColor);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}