using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public abstract class RememberEditor : Editor
    {
        private const string MSG_ACTIVE1 = "Disabled components can not be initialized on start.";
        private const string MSG_ACTIVE2 = "This component won't work until its first enabled.";

        protected RememberBase remember;

        protected virtual void OnEnable()
        {
            remember = target as RememberBase;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!Application.isPlaying && !remember.isActiveAndEnabled)
            {
                EditorGUILayout.HelpBox(
                    string.Format("{0} {1}", MSG_ACTIVE1, MSG_ACTIVE2),
                    MessageType.Warning
                );
            }

            string comment = Comment();
            if (!string.IsNullOrEmpty(comment))
            {
                EditorGUILayout.HelpBox(comment, MessageType.None);
            }

            OnPaint();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            GlobalEditorID.Paint(remember);
        }

        protected abstract void OnPaint();
        protected abstract string Comment();
    }
}