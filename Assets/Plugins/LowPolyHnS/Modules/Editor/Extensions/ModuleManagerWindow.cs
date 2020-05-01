using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.ModuleManager
{
    public class ModuleManagerWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "Module Manager";

        private const float WINDOW_W = 700f;
        private const float WINDOW_H = 500f;
        public const float WINDOW_SIDE_WIDTH = 200f;
        public const float WINDOW_SIDE_HEIGHT = 500f;

        public static ModuleManagerWindow WINDOW { get; private set; }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Vector2 contentScroll = Vector2.zero;
        private Vector2 sidebarScroll = Vector2.zero;
        private GUIStyle sidebarStyle = GUIStyle.none;
        private bool stylesInitialized = false;

        public int sidebarIndex = 0;

        // INITIALIZERS: --------------------------------------------------------------------------

        [MenuItem("LowPolyHnS/Module Manager %&m")]
        public static void OpenModuleManager()
        {
            Rect windowRect = new Rect(0f, 0f, WINDOW_W, WINDOW_H);
            WINDOW = GetWindowWithRect<ModuleManagerWindow>(
                windowRect, true, WINDOW_TITLE, true
            );

            ModuleManager.SetDirty();
            WINDOW.Show();

#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += ModuleManager.SetDirty;
#else
            EditorApplication.hierarchyWindowChanged += ModuleManager.SetDirty;
#endif
        }

        private static void CloseModuleManager()
        {
            if (WINDOW != null)
            {
                WINDOW.Close();
            }
        }

        private void OnDestroy()
        {
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged -= ModuleManager.SetDirty;
#else
            EditorApplication.hierarchyWindowChanged -= ModuleManager.SetDirty;
#endif
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void OnGUI()
        {
            InitializeStyles();
            if (WINDOW == null)
            {
                OpenModuleManager();
            }

            EditorGUILayout.BeginHorizontal();
            sidebarScroll = EditorGUILayout.BeginScrollView(
                sidebarScroll,
                false, false,
                GUIStyle.none,
                GUIStyle.none,
                sidebarStyle,
                GUILayout.MinWidth(WINDOW_SIDE_WIDTH),
                GUILayout.MaxWidth(WINDOW_SIDE_WIDTH),
                GUILayout.ExpandHeight(true)
            );

            PaintSidebar();

            EditorGUILayout.EndScrollView();
            Rect borderRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
            EditorGUI.DrawTextureAlpha(borderRect, Texture2D.blackTexture);

            EditorGUILayout.BeginVertical();

            PaintContent();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void PaintSidebar()
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(WINDOW_SIDE_WIDTH));
            ModuleManagerSidebar.PaintHeader();
            ModuleManagerSidebar.PaintSidebarProjects();
            EditorGUILayout.EndVertical();
        }

        private void PaintContent()
        {
            PaintToolbar();
            contentScroll = EditorGUILayout.BeginScrollView(
                contentScroll,
                GUIStyle.none,
                GUI.skin.verticalScrollbar
            );

            EditorGUILayout.Space();

            ModuleManifest[] manifests = ModuleManager.GetProjectManifests();
            if (sidebarIndex < 0 || sidebarIndex >= manifests.Length)
            {
                ModuleManagerContent.PaintContentMessage();
            }
            else
            {
                ModuleManifest manifest = manifests[sidebarIndex];
                ModuleManagerContent.PaintProjectModule(manifest);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        private void PaintToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                ModuleManager.Refresh();
            }

            EditorGUILayout.EndHorizontal();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            Texture2D texture = new Texture2D(1, 1);
            if (EditorGUIUtility.isProSkin) texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.35f));
            else texture.SetPixel(0, 0, new Color(256f, 256f, 256f, 0.5f));

            texture.alphaIsTransparency = true;
            texture.Apply();

            sidebarStyle = new GUIStyle();
            sidebarStyle.normal.background = texture;
            sidebarStyle.margin = new RectOffset(0, 0, 0, 0);
        }
    }
}