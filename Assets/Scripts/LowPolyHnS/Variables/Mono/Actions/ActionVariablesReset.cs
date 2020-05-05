using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionVariablesReset : IAction
    {
        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            VariablesManager.Reset();
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        private const string HELPBOX = "Reset all variable values";

        public static new string NAME = "Variables/Variables Reset";
        private const string NODE_TITLE = "Reset variables";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(HELPBOX, MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}