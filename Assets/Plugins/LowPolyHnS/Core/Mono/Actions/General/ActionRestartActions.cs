﻿using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionRestartActions : IAction
    {
        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            yield return int.MinValue;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "General/Restart Actions";
        private const string NODE_TITLE = "Restart the action";
        private const string MESSAGE = "Restart the execution of this Actions component";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild()
        {
        }

        protected override void OnDisableEditorChild()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(MESSAGE, MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}