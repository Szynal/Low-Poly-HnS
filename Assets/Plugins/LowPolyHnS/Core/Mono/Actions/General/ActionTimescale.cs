using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTimescale : IAction
    {
        public NumberProperty timeScale = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float timeScaleValue = timeScale.GetValue(target);
            TimeManager.Instance.SetTimeScale(timeScaleValue);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Time Scale";
        private const string NODE_TITLE = "Change Time Scale to {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTimeScale;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, timeScale);
        }

        protected override void OnEnableEditorChild()
        {
            spTimeScale = serializedObject.FindProperty("timeScale");
        }

        protected override void OnDisableEditorChild()
        {
            spTimeScale = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTimeScale);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}