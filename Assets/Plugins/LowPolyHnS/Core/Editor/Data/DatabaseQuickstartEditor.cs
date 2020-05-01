using System;
using System.IO;
using LowPolyHnS.ModuleManager;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(DatabaseQuickstart))]
    public class DatabaseQuickstartEditor : IDatabaseEditor
    {
        private class PageData
        {
            private const string TEXTURE_HEADER_PATH = "Assets/Plugins/LowPolyHnS/Extra/Icons/Quickstart/";

            public Action<Rect, Texture2D> paint;
            public Texture2D header;

            public PageData(Action<Rect, Texture2D> paint, string headerName)
            {
                string headerPath = Path.Combine(TEXTURE_HEADER_PATH, headerName);
                header = AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);
                this.paint = paint;
            }
        }

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private const float CONTROL_MARGIN_BOTTOM = 20.0f;
        private const float CONTROL_DOT_WIDTH = 20f;
        private const float CONTROL_BTN_WIDTH = 80.0f;
        private const float HEADER_IMAGES_AR = 900f / 500f;

        private AnimFloat animPagesIndex;
        private PageData[] pages;

        private GUIStyle titleStyle;
        private GUIStyle contentStyle;

        private float availableWidth;
        private float availableHeight;

        // INITIALIZE: -------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            animPagesIndex = new AnimFloat(0.0f, () => { Repaint(); });
            pages = new[]
            {
                new PageData(OnPaintWelcome, "welcome.png"),
                new PageData(OnPaintTutorials, "tutorials.png"),
                new PageData(OnPaintDocumentation, "documentation.png"),
                new PageData(OnPaintExamples, "examples.png"),
                new PageData(OnPaintModules, "modules.png"),
                new PageData(OnPaintStore, "hub.png")
            };
        }

        private void InitializeStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.boldLabel);
                titleStyle.fontSize = 13;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (contentStyle == null)
            {
                contentStyle = new GUIStyle(EditorStyles.helpBox);
                contentStyle.padding = new RectOffset(10, 10, 10, 10);
                contentStyle.alignment = TextAnchor.MiddleCenter;
                contentStyle.richText = true;
                contentStyle.wordWrap = true;
            }
        }

        // OVERRIDE METHODS: -------------------------------------------------------------------------------------------

        public override string GetName()
        {
            return "Quickstart";
        }

        public override int GetPanelWeight()
        {
            return 99;
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is only accessable through the Preferences Panel",
                MessageType.Info);
            if (GUILayout.Button("Open Preferences Window"))
            {
                PreferencesWindow.OpenWindow();
            }
        }

        public override void OnPreferencesWindowGUI()
        {
            serializedObject.Update();

            InitializeStyles();
            PaintPagesView();

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintPagesView()
        {
            Rect availableSpace = GUILayoutUtility.GetRect(
                GUIContent.none,
                GUIStyle.none,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            float areaWidth = availableWidth;
            float areaHeight = availableHeight;

            if (Event.current.type == EventType.Repaint)
            {
                availableWidth = availableSpace.width;
                availableHeight = availableSpace.height;
            }


            if (areaWidth <= Mathf.Epsilon || areaHeight <= Mathf.Epsilon) return;

            GUILayout.BeginArea(new Rect(
                animPagesIndex.value * -areaWidth,
                0,
                areaWidth * pages.Length,
                areaHeight
            ));

            for (int i = 0; i < pages.Length; ++i)
            {
                Rect bounds = new Rect(
                    i * areaWidth,
                    0.0f,
                    areaWidth,
                    areaHeight
                );

                pages[i].paint(bounds, pages[i].header);
            }

            GUILayout.EndArea();
            PaintControls();
        }

        private void PaintControls()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(Mathf.Approximately(animPagesIndex.target, 0.0f));
            if (GUILayout.Button("Previous", CoreGUIStyles.GetButtonLeft()))
            {
                float targ = animPagesIndex.target - 1.0f;
                if (targ < 0.0f) targ = 0.0f;
                animPagesIndex.target = targ;
            }

            EditorGUI.EndDisabledGroup();

            for (int i = 0; i < pages.Length; ++i)
            {
                GUIStyle dotStyle = Mathf.Approximately(animPagesIndex.target, i)
                    ? CoreGUIStyles.GetToggleButtonMidOn()
                    : CoreGUIStyles.GetToggleButtonMidOff();

                if (GUILayout.Button((i + 1).ToString(), dotStyle))
                {
                    animPagesIndex.target = i;
                }
            }

            EditorGUI.BeginDisabledGroup(Mathf.Approximately(animPagesIndex.target, pages.Length - 1.0f));
            if (GUILayout.Button("Next", CoreGUIStyles.GetButtonRight()))
            {
                float targ = animPagesIndex.target + 1.0f;
                if (targ > pages.Length - 1.0f) targ = pages.Length - 1.0f;
                animPagesIndex.target = targ;
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        // PAGES METHODS: ----------------------------------------------------------------------------------------------

        private Rect OnPaintPage(Rect bounds, string title, Texture2D header, float heightReserve = 80f)
        {
            Rect titleRect = GUILayoutUtility.GetRect(bounds.width, 50f);
            EditorGUI.LabelField(titleRect, title, titleStyle);


            Rect headerRect = GUILayoutUtility.GetRect(bounds.width, bounds.width / HEADER_IMAGES_AR);
            EditorGUI.DrawPreviewTexture(headerRect, header);

            Rect contentRect = GUILayoutUtility.GetRect(bounds.width, heightReserve);
            return new Rect(
                contentRect.x + 20f,
                contentRect.y + 20f,
                contentRect.width - 40f,
                contentRect.height - 20f
            );
        }

        private void OnPaintWelcome(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            Rect contentRect = OnPaintPage(bounds, "WELCOME TO LowPolyHnS", header);

            string content =
                "Follow these simple steps and become a pro with <b>LowPolyHnS</b> in less than 15 minutes";
            EditorGUI.LabelField(contentRect, content, contentStyle);
            GUILayout.EndArea();
        }

        private void OnPaintTutorials(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string videoTutorialsURL =
                "https://www.youtube.com/watch?v=IG8GXAAih2Q&list=PL7FyK0gfdpCbxMrWIV9B2xQiExkiZbpa5";
            string content = "Watch the <b>Getting Started Video Tutorials</b>";

            Rect contentRect = OnPaintPage(bounds, "GET STARTED IN 15 MINUTES", header);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "Watch playlist"))
            {
                Application.OpenURL(videoTutorialsURL);
            }

            GUILayout.EndArea();
        }

        private void OnPaintDocumentation(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string documentationURL = "https://docs.LowPolyHnS.io";
            string content = "Take a look at our beautifully hand-crafted <b>Documentation</b>";

            Rect contentRect = OnPaintPage(bounds, "DOCUMENTATION", header);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "docs.LowPolyHnS.io"))
            {
                Application.OpenURL(documentationURL);
            }

            GUILayout.EndArea();
        }

        private void OnPaintExamples(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string content = "Learn from the <b>Example Scenes</b>";

            Rect contentRect = OnPaintPage(bounds, "EXAMPLE SCENES", header);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "See Example Scenes"))
            {
                string scenePath = "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-Hub.unity";
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (sceneAsset != null)
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
                else
                {
                    ModuleManagerWindow.OpenModuleManager();
                }
            }

            GUILayout.EndArea();
        }

        private void OnPaintModules(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string content = "Check out the <b>Module Manager</b>";
            Rect contentRect = OnPaintPage(bounds, "MODULE MANAGER", header);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "Module Manager"))
            {
                ModuleManagerWindow.OpenModuleManager();
            }

            GUILayout.EndArea();
        }

        private void OnPaintStore(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string storeURL = "https://hub.LowPolyHnS.io";
            string content = "Visit <b>LowPolyHnS Hub</b>";

            Rect contentRect = OnPaintPage(bounds, "LowPolyHnS HUB", header);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "hub.LowPolyHnS.io"))
            {
                Application.OpenURL(storeURL);
            }

            GUILayout.EndArea();
        }
    }

    // ON INITIALIZE SHOW PREFERENCES: ---------------------------------------------------------------------------------

    [InitializeOnLoad]
    public class DatabaseQuickstartEditorOnLoad
    {
        private const string KEY_PREFERENCES_STARTUP = "show-quickstart-preferences-on-startup";

        static DatabaseQuickstartEditorOnLoad()
        {
            EditorApplication.update += Start;
        }

        private static void Start()
        {
            EditorApplication.update -= Start;
            if (EditorPrefs.GetBool(KEY_PREFERENCES_STARTUP, true))
            {
                EditorPrefs.SetBool(KEY_PREFERENCES_STARTUP, false);

                PreferencesWindow.SetSidebarIndex(0);
                PreferencesWindow.OpenWindow();
            }
        }
    }
}