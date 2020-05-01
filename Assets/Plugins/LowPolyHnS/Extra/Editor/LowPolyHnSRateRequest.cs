using System;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core.Rate
{
    public class LowPolyHnSRateRequest : EditorWindow
    {
        private const string KEY_PLAY_TIMES = "gc-unity-editor-open-count";
        private const string KEY_FIRST_TIME = "gc-unity-editor-first-time";
        private const string KEY_ALREADY_SN = "gc-unity-editor-already-se";

        private const string PATH_LOGO = "Assets/Plugins/LowPolyHnS/Extra/Icons/Rate/Logo@{0}x.png";
        private const string PATH_REVIEW = "Assets/Plugins/LowPolyHnS/Extra/Icons/Rate/IconReview.png";

        private const string MSG_TITLE = "Thank you for using LowPolyHnS!";

        private const string MSG_TEXT1 = "We hate to disturb you like this, but consider " +
                                         "leaving a review in the Asset Store.\n\nSpreading the word allow us to " +
                                         "keep developing LowPolyHnS and adding exciting new features.";

        private static readonly GUIContent MSG_TEXT2 = new GUIContent(
            "This will only appear once and we will not bother you again. Happy game making!"
        );

        private static readonly Color COLOR_FOOTER = new Color(0, 0, 0, 0.1f);

        private static LowPolyHnSRateRequest Instance;
        private static readonly Vector2 WIN_SIZE = new Vector2(300, 400);

        private const int SPACE = 20;
        private const int LOGO_SIZE = 80;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool showFooter;
        private bool stylesInitialized;

        private GUIStyle styleTitle;
        private GUIStyle styleText;
        private GUIStyle styleFooter;
        private GUIStyle styleReview;
        private Texture2D textureLogo;
        private GUIContent gcReview;

        // INITIALIZERS: --------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            if (EditorPrefs.GetInt(KEY_ALREADY_SN, 0) != 0) return;
            EditorApplication.playModeStateChanged += OnPlaymodeChange;
        }

        private static void OnPlaymodeChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode) return;

            int count = EditorPrefs.GetInt(KEY_PLAY_TIMES, 0);
            EditorPrefs.SetInt(KEY_PLAY_TIMES, count + 1);

            string strDate = EditorPrefs.GetString(KEY_FIRST_TIME, string.Empty);
            if (string.IsNullOrEmpty(strDate))
            {
                strDate = DateTime.Now.ToString("d");
                EditorPrefs.SetString(KEY_FIRST_TIME, strDate);
            }

            DateTime date = DateTime.Parse(strDate);
            if ((DateTime.Now - date).Days >= 3 && count >= 10)
            {
                OpenWindow(true);
                EditorPrefs.SetInt(KEY_ALREADY_SN, 1);
            }
        }

        private void InitializeStyles()
        {
            styleTitle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            styleText = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(SPACE, SPACE, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                wordWrap = true
            };

            styleFooter = new GUIStyle(styleText)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(8, 8, 8, 8),
                alignment = TextAnchor.MiddleLeft,
                fontSize = EditorStyles.centeredGreyMiniLabel.fontSize
            };

            styleReview = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, SPACE / 2, SPACE / 2),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                richText = true
            };

            textureLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(
                string.Format(PATH_LOGO, EditorGUIUtility.pixelsPerPoint > 1f ? 2 : 1
                ));

            gcReview = new GUIContent(
                " Review in the <b>Unity Asset Store</b>",
                AssetDatabase.LoadAssetAtPath<Texture2D>(PATH_REVIEW)
            );

            stylesInitialized = true;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        [MenuItem("LowPolyHnS/Tools/Review...")]
        public static void OpenWindow()
        {
            OpenWindow(false);
        }

        public static void OpenWindow(bool showFooter)
        {
            Instance = GetWindow<LowPolyHnSRateRequest>(true, string.Empty, true);

            Instance.showFooter = showFooter;
            Instance.maxSize = WIN_SIZE;
            Instance.minSize = WIN_SIZE;
            Instance.ShowTab();
        }

        private void OnGUI()
        {
            if (!stylesInitialized)
            {
                InitializeStyles();
            }

            GUILayout.Space(SPACE);

            Rect rectLogoTotal = GUILayoutUtility.GetRect(WIN_SIZE.x, LOGO_SIZE);
            Rect rectLogo = new Rect(
                rectLogoTotal.x + (rectLogoTotal.width / 2f - LOGO_SIZE / 2f),
                rectLogoTotal.y,
                LOGO_SIZE,
                LOGO_SIZE
            );

            GUI.DrawTexture(rectLogo, textureLogo);
            GUILayout.Space(SPACE);

            Rect rectTitle = GUILayoutUtility.GetRect(WIN_SIZE.x, 20);
            EditorGUI.LabelField(rectTitle, MSG_TITLE, styleTitle);

            GUILayout.Space(SPACE);
            EditorGUILayout.LabelField(MSG_TEXT1, styleText);

            GUILayout.Space(SPACE);
            if (GUILayout.Button(gcReview, styleReview, GUILayout.Height(40)))
            {
                Application.OpenURL("https://LowPolyHnS.page.link/review");
                Close();
            }

            if (!showFooter) return;
            GUILayout.FlexibleSpace();

            Rect rectFooter = GUILayoutUtility.GetRect(MSG_TEXT2, styleFooter);

            EditorGUI.DrawRect(rectFooter, COLOR_FOOTER);
            EditorGUI.LabelField(rectFooter, MSG_TEXT2, styleFooter);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
    }
}