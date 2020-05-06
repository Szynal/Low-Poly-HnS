using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LowPolyHnS.Tools
{
    public class MixamoManager : EditorWindow
    {
        private static MixamoManager EDITOR;
        private static int WIDTH = 300;
        private static int HEIGHT = 500;
        private static int X;
        private static int Y;
        private static List<string> ALL_FILES = new List<string>();

        private static Settings SETTINGS = new Settings();
        private const string SETTINGS_PREFS_PATH = nameof(MixamoManager) + "_lastsettings";
        private static Color LINE_COLOR = new Color32(128, 128, 128, 64);

        [Serializable]
        private class Settings
        {
            public string Path = string.Empty;
            public bool RenameAnimClips = true;
            public bool RenameAnimClipsUnderscores = true;
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

        [MenuItem("LowPolyHnS/MixamoManager")]
        private static void ShowEditor()
        {
            EDITOR = GetWindow<MixamoManager>();
            CenterWindow();
            LoadSettings();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select directory"))
            {
                SETTINGS.Path = EditorUtility.OpenFolderPanel("Select directory with files", "", "");
            }

            if (GUILayout.Button("Reset settings"))
            {
                SETTINGS = new Settings();
            }

            if (GUILayout.Button("Save settings"))
            {
                SaveSettings();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label($"Path: {SETTINGS.Path} ", EditorStyles.boldLabel);
            GUILayout.Label($"Selected: {Selection.gameObjects.Length} assets", EditorStyles.boldLabel);

            bool pathvalid = !string.IsNullOrEmpty(SETTINGS.Path) && Directory.Exists(SETTINGS.Path);
            if (!pathvalid)
            {
                GUI.color = Color.red;
                GUILayout.Label("Path invalid", EditorStyles.boldLabel);
                GUI.color = Color.white;
            }

            DrawUILine();
            SETTINGS.RenameAnimClips =
                EditorGUILayout.BeginToggleGroup("Rename anim clips to filename", SETTINGS.RenameAnimClips);
            {
                SETTINGS.RenameAnimClipsUnderscores =
                    EditorGUILayout.Toggle("Spaces to underscores", SETTINGS.RenameAnimClipsUnderscores);
                SETTINGS.RenameAnimClipsTolower = EditorGUILayout.Toggle("To lower", SETTINGS.RenameAnimClipsTolower);
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
            GUILayout.BeginHorizontal();

            GUI.enabled = pathvalid;
            if (GUILayout.Button("Process directory"))
            {
                process_dir();
                SaveSettings();
            }

            GUI.enabled = Selection.gameObjects.Length > 0;
            if (GUILayout.Button("Process selected assets"))
            {
                processSelectedAssets();
                SaveSettings();
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private static void SaveSettings()
        {
            string json = EditorJsonUtility.ToJson(SETTINGS);
            EditorPrefs.SetString(SETTINGS_PREFS_PATH, json);
        }

        private static void LoadSettings()
        {
            SETTINGS = JsonUtility.FromJson<Settings>(EditorPrefs.GetString(SETTINGS_PREFS_PATH));
            if (SETTINGS == null)
                SETTINGS = new Settings();
        }

        public void process_dir()
        {
            DirSearch(SETTINGS.Path);

            if (ALL_FILES.Count > 0)
            {
                for (int i = 0; i < ALL_FILES.Count; i++)
                {
                    int idx = ALL_FILES[i].IndexOf("Assets");
                    string filename = Path.GetFileName(ALL_FILES[i]);
                    string asset = ALL_FILES[i].Substring(idx);
                    AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(
                        asset, typeof(AnimationClip));

                    var fileName = Path.GetFileNameWithoutExtension(ALL_FILES[i]);
                    var importer = (ModelImporter) AssetImporter.GetAtPath(asset);

                    EditorUtility.DisplayProgressBar($"Processing {ALL_FILES.Count} files", filename,
                        1f / ALL_FILES.Count * i);

                    RenameAndImport(importer, fileName);
                }
            }

            EditorUtility.DisplayProgressBar($"Processing {ALL_FILES.Count} files", "Saving assets", 1f);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        public void processSelectedAssets()
        {
            int count = Selection.gameObjects.Length;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Object asset = Selection.gameObjects[i];
                    string assetpath = AssetDatabase.GetAssetPath(asset);
                    AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(
                        assetpath, typeof(AnimationClip));

                    var fileName = asset.name;
                    var importer = (ModelImporter) AssetImporter.GetAtPath(assetpath);

                    EditorUtility.DisplayProgressBar($"Processing {count} files", fileName, 1f / count * i);

                    RenameAndImport(importer, fileName);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private void RenameAndImport(ModelImporter asset, string name)
        {
            ModelImporter modelImporter = asset;
            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

            if (SETTINGS.DisableMaterialImport)
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

            if (SETTINGS.SetRigToHumanoid)
                modelImporter.animationType = ModelImporterAnimationType.Human;

            if (SETTINGS.RigCustomAvatar != null)
                modelImporter.sourceAvatar = SETTINGS.RigCustomAvatar;

            if (SETTINGS.RenameAnimClipsUnderscores)
                name = name.Replace(' ', '_');

            if (SETTINGS.RenameAnimClipsTolower)
                name = name.ToLower();

            for (int i = 0; i < clipAnimations.Length; i++)
            {
                var clip = clipAnimations[i];

                if (SETTINGS.RenameAnimClips)
                    clip.name = name;
                if (SETTINGS.ChangeLoopAnimClips)
                {
                    clip.loopTime = SETTINGS.LoopAnimClipsTime;
                    clip.loopPose = SETTINGS.LoopAnimClipsPose;
                    if (SETTINGS.RootTransformRotation)
                    {
                        clip.lockRootRotation = SETTINGS.RootTransformRotationBakeInToPose;
                        clip.keepOriginalOrientation = SETTINGS.RootTransformRotationKeepOriginal;
                        clip.rotationOffset = SETTINGS.RootTransformRotationOffset;
                    }
                }
            }

            modelImporter.clipAnimations = clipAnimations;
            modelImporter.SaveAndReimport();
        }

        private static void CenterWindow()
        {
            EDITOR = GetWindow<MixamoManager>();
            X = (Screen.currentResolution.width - WIDTH) / 2;
            Y = (Screen.currentResolution.height - HEIGHT) / 2;
            EDITOR.position = new Rect(X, Y, WIDTH, HEIGHT);
            EDITOR.maxSize = new Vector2(WIDTH, HEIGHT);
            EDITOR.minSize = EDITOR.maxSize;
        }

        private static void DirSearch(string path)
        {
            string[] fileInfo = Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories);
            foreach (string file in fileInfo)
            {
                if (file.EndsWith(".fbx"))
                    ALL_FILES.Add(file);
            }
        }

        private static void DrawUILine(int thickness = 1, int padding = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, LINE_COLOR);
        }
    }
}