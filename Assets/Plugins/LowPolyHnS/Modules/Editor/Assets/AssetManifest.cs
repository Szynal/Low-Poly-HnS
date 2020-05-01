using System;
using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.ModuleManager
{
    [Serializable]
    public class AssetManifest : ScriptableObject
    {
        private const string RELATIVE_PATH = "Assets/{0}/{1}";
        private const string ASSET_PATH = "Plugins/LowPolyHnSData/Assets";
        private const string ASSET_NAME = "Manifest.asset";

        private const string PROP_MANIFESTS = "manifests";
        private const string PROP_IS_ENABLED = "isEnabled";
        private const string PROP_MODULE = "module";

        private static AssetManifest Instance;

        // PROPERTIES: ----------------------------------------------------------------------------

        public ModuleManifest[] manifests = new ModuleManifest[0];

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static AssetManifest GetInstance()
        {
            if (Instance != null) return Instance;

            AssetManifest manifest;
            string absPath = Path.Combine(Application.dataPath, Path.Combine(ASSET_PATH, ASSET_NAME));
            string relPath = string.Format(RELATIVE_PATH, ASSET_PATH, ASSET_NAME);

            if (File.Exists(absPath))
            {
                manifest = AssetDatabase.LoadAssetAtPath<AssetManifest>(relPath);
            }
            else
            {
                string dirPath = string.Format(RELATIVE_PATH, ASSET_PATH, "");
                LowPolyHnSUtilities.CreateFolderStructure(dirPath);

                manifest = CreateInstance<AssetManifest>();
                AssetDatabase.CreateAsset(manifest, relPath);
            }

            Instance = manifest;
            return Instance;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ModuleManifest[] GetManifests()
        {
            return manifests;
        }

        public void UpdateManifest(Module module)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty propManifests = serializedObject.FindProperty(PROP_MANIFESTS);

            int manifestIndex = GetManifestIndex(module);
            if (manifestIndex >= 0)
            {
                propManifests = propManifests.GetArrayElementAtIndex(manifestIndex);
                Module.UpdateModule(propManifests.FindPropertyRelative(PROP_MODULE), module);
            }
            else
            {
                manifestIndex = propManifests.arraySize;
                propManifests.InsertArrayElementAtIndex(manifestIndex);
                propManifests = propManifests.GetArrayElementAtIndex(manifestIndex);
                Module.UpdateModule(propManifests.FindPropertyRelative(PROP_MODULE), module);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public void RemoveModule(Module module)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty propManifests = serializedObject.FindProperty(PROP_MANIFESTS);

            int manifestIndex = GetManifestIndex(module);
            if (manifestIndex >= 0)
            {
                propManifests.DeleteArrayElementAtIndex(manifestIndex);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private int GetManifestIndex(Module module)
        {
            for (int i = 0; i < manifests.Length; ++i)
            {
                if (manifests[i].module.moduleID == module.moduleID) return i;
            }

            return -1;
        }
    }
}