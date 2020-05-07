using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class MixamoManager : EditorWindow
    {
        private const string WIN_TITLE = "Mixamo Preferences";
        private const string KEY_SIDEBAR_INDEX = "MixamoManager-preferences-index";

        private static MixamoManager EDITOR;
        private static List<string> ALL_FILES = new List<string>();

        private static Settings SETTINGS = new Settings();

        public const float SIDEBAR_WIDTH = 160.0f;
        private const string SETTINGS_PATH = nameof(MixamoManager) + "_lastsettings";
        private const string MIXAMO_LOGO = "Assets/EditorIcons/Mixamo/mixamo-logo.png";
        private static Texture2D IMAGE_MIXAMO_LOGO;

        private Vector2 scrollSidebar = Vector2.zero;
        private Vector2 scrollContent = Vector2.zero;
        private int sidebarIndex;

        private bool initStyles;
        private GUIStyle styleSidebar;
        private static Color LINE_COLOR = new Color32(128, 128, 128, 64);

        [Serializable]
        private class Settings
        {
            public string Path = string.Empty;
            public bool RenameAnimClips = true;
            public bool RenameAnimClipsUnderscores = true;
            public bool RenameAnimClipsSpaceTrim = true;
            public bool RenameAnimClipsTolower = true;
            public bool ChangeLoopAnimClips = true;
            public bool LoopAnimClipsTime = true;
            public bool LoopAnimClipsPose;
            public bool RootTransformRotation;
            public bool RootTransformRotationBakeInToPose;
            public bool RootTransformRotationKeepOriginal;
            public float RootTransformRotationOffset;

            public bool DisableMaterialImport = true;
            public bool Mirror;
            public bool SetRigToHumanoid = true;
            public Avatar RigCustomAvatar;
        }


        #region INITIALIZE METHODS

        private void OnEnable()
        {
            initStyles = false;
            ChangeSidebarIndex(EditorPrefs.GetInt(KEY_SIDEBAR_INDEX, 0));
            IMAGE_MIXAMO_LOGO = (Texture2D) AssetDatabase.LoadAssetAtPath(MIXAMO_LOGO, typeof(Texture2D));
        }

        #endregion

        #region GUI METHODS

        private void OnGUI()
        {
            if (EDITOR == null)
            {
                OpenWindow();
            }

            if (!initStyles)
            {
                InitializeStyles();
            }

            int currentSidebarIndex = sidebarIndex;
            if (currentSidebarIndex < 0)
            {
                currentSidebarIndex = 0;
                ChangeSidebarIndex(currentSidebarIndex);
            }

            EditorGUILayout.BeginHorizontal();

            PaintSidebar();
            PaintSettings();

            EditorGUILayout.EndHorizontal();


            Repaint();
        }


        private void InitializeStyles()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(
                0, 0, EditorGUIUtility.isProSkin
                    ? new Color(0f, 0f, 0f, 0.35f)
                    : new Color(256f, 256f, 256f, 0.5f));

            texture.alphaIsTransparency = true;
            texture.Apply();

            styleSidebar = new GUIStyle {normal = {background = texture}, margin = new RectOffset(0, 0, 0, 0)};
            initStyles = true;
        }

        private void ChangeSidebarIndex(int nextIndex)
        {
            sidebarIndex = nextIndex;
            EditorPrefs.SetInt(KEY_SIDEBAR_INDEX, sidebarIndex);

            string windowName = GetWindowTitle();
            titleContent = new GUIContent(windowName);
        }

        private void PaintSidebar()
        {
            scrollSidebar = EditorGUILayout.BeginScrollView(
                scrollSidebar,
                styleSidebar,
                GUILayout.MinWidth(SIDEBAR_WIDTH),
                GUILayout.MaxWidth(SIDEBAR_WIDTH),
                GUILayout.ExpandHeight(true)
            );

            if (GUILayout.Button(IMAGE_MIXAMO_LOGO, new GUIStyle(GUI.skin.label), GUILayout.Height(70f),
                GUILayout.Width(150f)))
            {
                Application.OpenURL("https://www.Mixamo.com/");
            }

            if (GUILayout.Button("Reset settings"))
            {
                SETTINGS = new Settings();
            }

            if (GUILayout.Button("Save settings"))
            {
                SaveSettings();
            }

            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            GUILayout.Label($"Selected: {Selection.gameObjects.Length} assets", EditorStyles.boldLabel,
                GUILayout.Width(130f));
            EditorGUILayout.EndHorizontal();

            GUI.enabled = Selection.gameObjects.Length > 0;

            if (GUILayout.Button("Process selected assets"))
            {
                ProcessSelectedAssets();
                SaveSettings();
            }


            EditorGUILayout.EndScrollView();
            Rect borderRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
            EditorGUI.DrawTextureAlpha(borderRect, Texture2D.blackTexture);
        }

        private void PaintSettings()
        {
            GUI.enabled = true;
            EditorGUILayout.BeginVertical();
            DrawUILine();

            SETTINGS.RenameAnimClips =
                EditorGUILayout.BeginToggleGroup("Rename anim clips to filename", SETTINGS.RenameAnimClips);
            {
                SETTINGS.RenameAnimClipsUnderscores =
                    EditorGUILayout.Toggle("Spaces to underscores", SETTINGS.RenameAnimClipsUnderscores);
                SETTINGS.RenameAnimClipsTolower = EditorGUILayout.Toggle("To lower", SETTINGS.RenameAnimClipsTolower);
                SETTINGS.RenameAnimClipsSpaceTrim =
                    EditorGUILayout.Toggle("Delete spaces", SETTINGS.RenameAnimClipsSpaceTrim);
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.Space(5);

            DrawUILine();
            SETTINGS.ChangeLoopAnimClips =
                EditorGUILayout.BeginToggleGroup("Change looping", SETTINGS.ChangeLoopAnimClips);
            {
                SETTINGS.LoopAnimClipsTime = EditorGUILayout.Toggle("Loop time", SETTINGS.LoopAnimClipsTime);
                SETTINGS.LoopAnimClipsPose = EditorGUILayout.Toggle("Loop pose", SETTINGS.LoopAnimClipsPose);
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.Space(5);

            DrawUILine();
            SETTINGS.RootTransformRotation =
                EditorGUILayout.BeginToggleGroup("Root transform rotation", SETTINGS.RootTransformRotation);
            {
                SETTINGS.RootTransformRotationBakeInToPose =
                    EditorGUILayout.Toggle("Bake into pose", SETTINGS.RootTransformRotationBakeInToPose);
                SETTINGS.RootTransformRotationKeepOriginal =
                    EditorGUILayout.Toggle("Keep original", SETTINGS.RootTransformRotationKeepOriginal);
                SETTINGS.RootTransformRotationOffset =
                    EditorGUILayout.FloatField("Offset", SETTINGS.RootTransformRotationOffset);
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.Space(5);

            GUILayout.Label("Misc", EditorStyles.boldLabel);
            DrawUILine();
            SETTINGS.SetRigToHumanoid = EditorGUILayout.Toggle("Set rig to humanoid", SETTINGS.SetRigToHumanoid);
            SETTINGS.DisableMaterialImport =
                EditorGUILayout.Toggle("Disable material import", SETTINGS.DisableMaterialImport);
            SETTINGS.Mirror = EditorGUILayout.Toggle("Mirror", SETTINGS.Mirror);
            SETTINGS.RigCustomAvatar =
                EditorGUILayout.ObjectField("Custom avatar", SETTINGS.RigCustomAvatar, typeof(Avatar), false) as Avatar;

            GUILayout.Space(30);
            DrawUILine();
        }

        #endregion

        public void ProcessSelectedAssets()
        {
            int count = Selection.gameObjects.Length;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var asset = Selection.gameObjects[i];
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    AnimationClip orgClip =
                        (AnimationClip) AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimationClip));

                    string fileName = asset.name;
                    ModelImporter importer = (ModelImporter) AssetImporter.GetAtPath(assetPath);

                    EditorUtility.DisplayProgressBar($"Processing {count} files", fileName, 1f / count * i);

                    RenameAndImport(importer, fileName);
                }
            }

            EditorUtility.ClearProgressBar();
        }


        private static void RenameAndImport(ModelImporter asset, string animName)
        {
            ModelImporter modelImporter = asset;
            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

            if (SETTINGS.DisableMaterialImport)
            {
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
            }

            if (SETTINGS.SetRigToHumanoid)
            {
                modelImporter.animationType = ModelImporterAnimationType.Human;
            }

            if (SETTINGS.RigCustomAvatar != null)
            {
                modelImporter.sourceAvatar = SETTINGS.RigCustomAvatar;
            }

            if (SETTINGS.RenameAnimClipsUnderscores)
            {
                animName = animName.Replace(' ', '_');
            }


            if (SETTINGS.RenameAnimClipsSpaceTrim)
            {
                animName = string.Join("", animName.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            }

            if (SETTINGS.RenameAnimClipsTolower)
            {
                animName = animName.ToLower();
            }

            foreach (ModelImporterClipAnimation clip in clipAnimations)
            {
                if (SETTINGS.RenameAnimClips)
                {
                    clip.name = animName;
                }

                if (!SETTINGS.ChangeLoopAnimClips)
                {
                    continue;
                }

                clip.loopTime = SETTINGS.LoopAnimClipsTime;
                clip.loopPose = SETTINGS.LoopAnimClipsPose;

                if (!SETTINGS.RootTransformRotation)
                {
                    continue;
                }

                clip.lockRootRotation = SETTINGS.RootTransformRotationBakeInToPose;
                clip.keepOriginalOrientation = SETTINGS.RootTransformRotationKeepOriginal;
                clip.rotationOffset = SETTINGS.RootTransformRotationOffset;
            }

            modelImporter.clipAnimations = clipAnimations;
            modelImporter.SaveAndReimport();
        }


        #region OPEN WINDOW SHORTCUT

        [MenuItem("LowPolyHnS/MixamoManager %&l")]
        public static MixamoManager OpenWindow()
        {
            MixamoManager window = GetWindow<MixamoManager>(true, GetWindowTitle(), true);
            LoadSettings();

            EDITOR = window;
            window.Show();
            return window;
        }

        public static void CloseWindow()
        {
            if (EDITOR == null)
            {
                return;
            }

            EDITOR.Close();
        }

        public static void OpenWindowTab(string tabName)
        {
            MixamoManager window = OpenWindow();
            tabName = tabName.ToLower();
        }

        #endregion


        private static string GetWindowTitle()
        {
            return WIN_TITLE;
        }


        private static void SaveSettings()
        {
            string json = EditorJsonUtility.ToJson(SETTINGS);
            EditorPrefs.SetString(SETTINGS_PATH, json);
        }

        private static void LoadSettings()
        {
            SETTINGS = JsonUtility.FromJson<Settings>(EditorPrefs.GetString(SETTINGS_PATH)) ?? new Settings();
        }

        private static void DrawUILine(int thickness = 1, int padding = 5)
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding / 2;
            rect.x -= 2;
            rect.width += 6;
            EditorGUI.DrawRect(rect, LINE_COLOR);
        }
    }
}