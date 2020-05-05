using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCanvasGroup : IAction
    {
        public CanvasGroup canvasGroup;

        [Range(0.0f, 5.0f)] public float duration = 0.5f;


        public NumberProperty alpha = new NumberProperty(0.0f);
        public BoolProperty interactible = new BoolProperty(true);
        public BoolProperty blockRaycasts = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (duration <= 0.0f)
            {
                canvasGroup.alpha = alpha.GetValue(target);
                canvasGroup.interactable = interactible.GetValue(target);
                canvasGroup.blocksRaycasts = blockRaycasts.GetValue(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = interactible.GetValue(target);
                canvasGroup.blocksRaycasts = blockRaycasts.GetValue(target);
                float targetAlpha = alpha.GetValue(target);

                if (duration > 0.0f)
                {
                    float currentAlpha = canvasGroup.alpha;
                    float startTime = Time.unscaledTime;

                    WaitUntil waitUntil = new WaitUntil(() =>
                    {
                        float t = (Time.unscaledTime - startTime) / duration;
                        canvasGroup.alpha = Mathf.Lerp(
                            currentAlpha,
                            targetAlpha,
                            t
                        );

                        return t > 1.0f;
                    });

                    yield return waitUntil;
                }

                canvasGroup.alpha = targetAlpha;
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "UI/Canvas Group";
        private const string NODE_TITLE = "Change CanvasGroup settings";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCanvasGroup;
        private SerializedProperty spDuration;
        private SerializedProperty spAlpha;
        private SerializedProperty spInteractible;
        private SerializedProperty spBlockRaycasts;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
            spCanvasGroup = serializedObject.FindProperty("canvasGroup");
            spDuration = serializedObject.FindProperty("duration");
            spAlpha = serializedObject.FindProperty("alpha");
            spInteractible = serializedObject.FindProperty("interactible");
            spBlockRaycasts = serializedObject.FindProperty("blockRaycasts");
        }

        protected override void OnDisableEditorChild()
        {
            spCanvasGroup = null;
            spDuration = null;
            spAlpha = null;
            spInteractible = null;
            spBlockRaycasts = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCanvasGroup);
            EditorGUILayout.PropertyField(spDuration);
            EditorGUILayout.PropertyField(spAlpha);
            EditorGUILayout.PropertyField(spInteractible);
            EditorGUILayout.PropertyField(spBlockRaycasts);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}