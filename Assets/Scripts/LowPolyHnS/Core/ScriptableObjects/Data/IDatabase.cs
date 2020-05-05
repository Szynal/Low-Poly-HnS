using System;
using System.IO;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public abstract class IDatabase : ScriptableObject
    {
        private const string DATABASE_RESOURCE_PATH = "LowPolyHnS/Databases";

        // MAIN METHODS: --------------------------------------------------------------------------

        public static T LoadDatabaseCopy<T>() where T : IDatabase
        {
            T database = LoadDatabase<T>();
            return Instantiate(database);
        }

        public static T LoadDatabase<T>(bool onlyLoad = false) where T : IDatabase
        {
            string path = Path.Combine(
                DATABASE_RESOURCE_PATH,
                GetAssetFilename(typeof(T), false)
            );

            T database = Resources.Load<T>(path);
            if (database == null && !onlyLoad) database = CreateInstance<T>();

            return database;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static string GetAssetFilename(Type type, bool withExtension)
        {
            string[] names = type.Name.Split('.');

            string name = names[names.Length - 1];
            if (withExtension)
            {
                name = string.Format("{0}.asset", name);
            }

            return name;
        }

        // EDITOR METHODS: ------------------------------------------------------------------------

#if UNITY_EDITOR

        protected static void Setup<T>() where T : IDatabase
        {
            EditorApplication.update += SetupDeferred<T>;
        }

        private static void SetupDeferred<T>() where T : IDatabase
        {
            EditorApplication.update -= SetupDeferred<T>;

            T database = CreateInstance<T>();
            string assetPath = database.GetAssetPath();
            IDatabase asset = AssetDatabase.LoadAssetAtPath<IDatabase>(assetPath);

            if (asset == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(assetPath);
                AssetDatabase.Refresh();

                asset = CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        // VIRTUAL & ABSTRACT METHODS: ------------------------------------------------------------

        protected virtual string GetProjectPath()
        {
            return "Assets/Scripts/LowPolyHnSData/Core/Resources";
        }

        protected virtual string GetResourcePath()
        {
            return "LowPolyHnS/Databases";
        }

        protected virtual string GetAssetPath()
        {
            string assetPath = Path.Combine(
                GetProjectPath(),
                GetResourcePath()
            );

            return Path.Combine(
                assetPath,
                GetAssetFilename(GetType(), true)
            );
        }

        public virtual int GetSidebarPriority()
        {
            return 50;
        }

#endif
    }
}