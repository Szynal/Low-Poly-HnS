using UnityEngine;
using UnityEngine.Playables;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTimeline : IAction
    {
        public enum Operation
        {
            Play,
            Pause,
            Stop
        }

        public PlayableDirector director;
        public Operation operation = Operation.Play;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (director != null)
            {
                switch (operation)
                {
                    case Operation.Play:
                        director.Play();
                        break;
                    case Operation.Pause:
                        director.Pause();
                        break;
                    case Operation.Stop:
                        director.Stop();
                        break;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Timeline";
        private const string NODE_TITLE = "{0} Timeline {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spDirector;
        private SerializedProperty spOperation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                operation.ToString(),
                director == null ? "none" : director.gameObject.name
            );
        }

        protected override void OnEnableEditorChild()
        {
            spDirector = serializedObject.FindProperty("director");
            spOperation = serializedObject.FindProperty("operation");
        }

        protected override void OnDisableEditorChild()
        {
            spDirector = null;
            spOperation = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spDirector);
            EditorGUILayout.PropertyField(spOperation);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}