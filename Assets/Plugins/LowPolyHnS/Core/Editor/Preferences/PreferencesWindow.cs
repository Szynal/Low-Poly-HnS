using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class PreferencesWindow : EditorWindow
    {
        private const string WIN_TITLE = "LowPolyHnS Preferences";
        private const string KEY_SIDEBAR_INDEX = "LowPolyHnS-preferences-index";

        public const float WIN_WIDTH = 800.0f;
        public const float WIN_HEIGHT = 700.0f;
        public const float SIDEBAR_WIDTH = 150.0f;

        private static PreferencesWindow Instance;

        private class DatabaseInfo : IComparable<DatabaseInfo>
        {
            public string name;
            public IDatabase data;
            public IDatabaseEditor dataEditor;

            public DatabaseInfo(IDatabase data)
            {
                this.data = data;
                dataEditor = Editor.CreateEditor(this.data) as IDatabaseEditor;
                name = dataEditor.GetName();
            }

            public int CompareTo(DatabaseInfo value)
            {
                int valueA = dataEditor.GetPanelWeight();
                int valueB = value.dataEditor.GetPanelWeight();
                return valueA.CompareTo(valueB);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Vector2 scrollSidebar = Vector2.zero;
        private Vector2 scrollContent = Vector2.zero;
        private int sidebarIndex;

        private bool initStyles;
        private GUIStyle styleSidebar;

        private static List<DatabaseInfo> DATABASES = new List<DatabaseInfo>();

        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void OnEnable()
        {
            initStyles = false;
            ChangeSidebarIndex(EditorPrefs.GetInt(KEY_SIDEBAR_INDEX, 0));
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        private void OnGUI()
        {
            if (Instance == null)
            {
                OpenWindow();
            }

            if (!initStyles) InitializeStyles();

            int currentSidebarIndex = sidebarIndex;
            if (currentSidebarIndex < 0)
            {
                currentSidebarIndex = 0;
                ChangeSidebarIndex(currentSidebarIndex);
            }
            else if (currentSidebarIndex >= DATABASES.Count)
            {
                currentSidebarIndex = DATABASES.Count - 1;
                ChangeSidebarIndex(currentSidebarIndex);
            }

            EditorGUILayout.BeginHorizontal();

            PaintSidebar(currentSidebarIndex);

            EditorGUILayout.BeginVertical();
            PaintToolbar(currentSidebarIndex);
            PaintContent(currentSidebarIndex);
            EditorGUILayout.EndVertical();

            Repaint();
            EditorGUILayout.EndHorizontal();
        }

        private void PaintSidebar(int currentSidebarIndex)
        {
            scrollSidebar = EditorGUILayout.BeginScrollView(
                scrollSidebar,
                styleSidebar,
                GUILayout.MinWidth(SIDEBAR_WIDTH),
                GUILayout.MaxWidth(SIDEBAR_WIDTH),
                GUILayout.ExpandHeight(true)
            );

            for (int i = DATABASES.Count - 1; i >= 0; --i)
            {
                if (DATABASES[i].data == null) DATABASES.RemoveAt(i);
            }

            for (int i = 0; i < DATABASES.Count; ++i)
            {
                Rect itemRect = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetItemPreferencesSidebar());

                if (Event.current.type == EventType.MouseDown &&
                    itemRect.Contains(Event.current.mousePosition))
                {
                    ChangeSidebarIndex(i);
                }

                bool isActive = currentSidebarIndex == i;

                if (Event.current.type == EventType.Repaint)
                {
                    string text = DATABASES[i].name;
                    CoreGUIStyles.GetItemPreferencesSidebar().Draw(itemRect, text, isActive, isActive, false, false);
                }
            }

            EditorGUILayout.EndScrollView();

            Rect borderRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
            EditorGUI.DrawTextureAlpha(borderRect, Texture2D.blackTexture);
        }

        private void LoadDatabases()
        {
            List<IDatabase> databases = LowPolyHnSUtilities.FindAssetsByType<IDatabase>();
            int databasesCount = databases.Count;

            DATABASES = new List<DatabaseInfo>();
            for (int i = 0; i < databasesCount; ++i)
            {
                DATABASES.Add(new DatabaseInfo(databases[i]));
            }

            DATABASES.Sort((x, y) =>
            {
                int valueX = x.data.GetSidebarPriority();
                int valueY = y.data.GetSidebarPriority();
                return valueX.CompareTo(valueY);
            });
        }

        private void PaintToolbar(int currentSidebarIndex)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void PaintContent(int currentSidebarIndex)
        {
            if (DATABASES.Count == 0) return;
            scrollContent = EditorGUILayout.BeginScrollView(
                scrollContent,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            EditorGUILayout.Space();
            DATABASES[currentSidebarIndex].dataEditor.OnPreferencesWindowGUI();
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }

        private void InitializeStyles()
        {
            Texture2D texture = new Texture2D(1, 1);
            if (EditorGUIUtility.isProSkin) texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.35f));
            else texture.SetPixel(0, 0, new Color(256f, 256f, 256f, 0.5f));

            texture.alphaIsTransparency = true;
            texture.Apply();

            styleSidebar = new GUIStyle();
            styleSidebar.normal.background = texture;
            styleSidebar.margin = new RectOffset(0, 0, 0, 0);

            initStyles = true;
        }

        // OPEN WINDOW SHORTCUT: ------------------------------------------------------------------

        [MenuItem("LowPolyHnS/Preferences %&k")]
        public static PreferencesWindow OpenWindow()
        {
            PreferencesWindow window = GetWindow<PreferencesWindow>(
                true,
                GetWindowTitle(),
                true
            );

            Instance = window;
            window.LoadDatabases();
            window.Show();
            return window;
        }

        public static void CloseWindow()
        {
            if (Instance == null) return;
            Instance.Close();
        }

        public static void OpenWindowTab(string tabName)
        {
            PreferencesWindow window = OpenWindow();

            tabName = tabName.ToLower();
            for (int i = 0; i < DATABASES.Count; ++i)
            {
                if (DATABASES[i].name.ToLower() == tabName)
                {
                    window.sidebarIndex = i;
                    break;
                }
            }
        }

        private void ChangeSidebarIndex(int nextIndex)
        {
            sidebarIndex = nextIndex;
            EditorPrefs.SetInt(KEY_SIDEBAR_INDEX, sidebarIndex);

            string windowName = GetWindowTitle();
            titleContent = new GUIContent(windowName);
        }

        private static string GetWindowTitle()
        {
            return WIN_TITLE;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void SetSidebarIndex(int index)
        {
            EditorPrefs.SetInt(KEY_SIDEBAR_INDEX, index);
            if (Instance != null)
            {
                Instance.ChangeSidebarIndex(index);
            }
        }
    }
}