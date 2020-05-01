﻿using System.Collections.Generic;
using UnityEditor;

namespace LowPolyHnS.Core
{
    internal static class LowPolyHnSExamples
    {
        private static readonly string[] SCENES =
        {
            "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-Intro.unity",
            "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-Hub.unity",
            "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-1.unity",
            "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-2.unity",
            "Assets/Plugins/LowPolyHnS/Examples/Scenes/Example-3.unity"
        };

        // SETUP METHODS: -------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes;
            editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            for (int i = 0; i < SCENES.Length; ++i)
            {
                bool sceneInBuildSettings = false;
                for (int j = 0; j < editorBuildSettingsScenes.Count; ++j)
                {
                    if (editorBuildSettingsScenes[j].path == SCENES[i])
                    {
                        sceneInBuildSettings = true;
                    }
                }

                if (sceneInBuildSettings) continue;

                EditorBuildSettingsScene scene = new EditorBuildSettingsScene(SCENES[i], true);
                if (!editorBuildSettingsScenes.Contains(scene))
                {
                    editorBuildSettingsScenes.Add(scene);
                }
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }
    }
}