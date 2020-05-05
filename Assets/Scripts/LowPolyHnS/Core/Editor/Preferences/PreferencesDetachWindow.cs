using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class PreferencesDetachWindow : EditorWindow
    {
        private static Dictionary<string, PreferencesDetachWindow> WINDOWS;

        // PROPERTIES: ----------------------------------------------------------------------------

        private IDatabaseEditor editor;
        private Vector2 scroll = Vector2.zero;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Create(IDatabaseEditor editor)
        {
            if (WINDOWS == null) WINDOWS = new Dictionary<string, PreferencesDetachWindow>();
            if (WINDOWS.ContainsKey(editor.GetName()))
            {
                if (WINDOWS[editor.GetName()] != null)
                {
                    WINDOWS[editor.GetName()].Close();
                }

                WINDOWS.Remove(editor.GetName());
            }

            string name = editor.GetName();
            PreferencesDetachWindow window = CreateInstance<PreferencesDetachWindow>();
            window.titleContent = new GUIContent(name);

            WINDOWS.Add(name, window);
            window.editor = editor;
            window.Show();
            window.Focus();
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        private void OnGUI()
        {
            if (editor == null)
            {
                Close();
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.Space();
            editor.OnInspectorGUI();

            EditorGUILayout.EndScrollView();
            Repaint();
        }
    }
}