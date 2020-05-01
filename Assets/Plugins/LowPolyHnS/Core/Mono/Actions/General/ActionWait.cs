using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionWait : IAction
    {
        public float waitTime = 0.0f;
        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            forceStop = false;

            float stopTime = Time.time + waitTime;
            WaitUntil waitUntil = new WaitUntil(() => Time.time > stopTime || forceStop);

            yield return waitUntil;
            yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Wait";
        private const string NODE_TITLE = "Wait {0} second{1}";

        private static readonly GUIContent GUICONTENT_WAITTIME = new GUIContent("Time to wait (s)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spWaitTime;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, waitTime, waitTime == 1f ? "" : "s");
        }

        protected override void OnEnableEditorChild()
        {
            spWaitTime = serializedObject.FindProperty("waitTime");
        }

        protected override void OnDisableEditorChild()
        {
            spWaitTime = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spWaitTime, GUICONTENT_WAITTIME);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}