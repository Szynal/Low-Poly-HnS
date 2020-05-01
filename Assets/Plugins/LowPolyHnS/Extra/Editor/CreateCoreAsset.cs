using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class CreateCoreAsset
    {
        private const string CORE_TEMPLATES_PATH = "Assets/Plugins/LowPolyHnS/Extra/ScriptTemplates/";

        private const string ACTION_TEMPLATE_FILE_NORM = "ActionTemplate.cs";
        private const string ACTION_TEMPLATE_FILE_SIMP = "SimpleActionTemplate.cs";

        private const string CONDITION_TEMPLATE_FILE_NORM = "ConditionTemplate.cs";
        private const string CONDITION_TEMPLATE_FILE_SIMP = "SimpleConditionTemplate.cs";

        private const string IGNITER_TEMPLATE_FILE = "IgniterTemplate.cs";

        private const string ACTION_EXAMPLE_NAME = "NewAction.cs";
        private const string CONDITION_EXAMPLE_NAME = "NewCondition.cs";
        private const string IGNITER_EXAMPLE_NAME = "NewIgniter.cs";

        // CONTENT DATA: --------------------------------------------------------------------------

        private const string REPLACE_CLASS_NAME = "__CLASS_NAME__";
        private const string REPLACE_HEADER = "/* ##HEADER##\n";
        private const string REPLACE_FOOTER = "\n##FOOTER## */";

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        [MenuItem("Assets/Create/LowPolyHnS/Developer/C# Simple Action", false, 50)]
        public static void CreateSimpleActionTemplate()
        {
            CreateGeneric(ACTION_TEMPLATE_FILE_SIMP, ACTION_EXAMPLE_NAME);
        }

        [MenuItem("Assets/Create/LowPolyHnS/Developer/C# Complete Action", false, 50)]
        public static void CreateCompleteActionTemplate()
        {
            CreateGeneric(ACTION_TEMPLATE_FILE_NORM, ACTION_EXAMPLE_NAME);
        }

        [MenuItem("Assets/Create/LowPolyHnS/Developer/C# Simple Condition", false, 100)]
        public static void CreateSimpleConditionTemplate()
        {
            CreateGeneric(CONDITION_TEMPLATE_FILE_SIMP, CONDITION_EXAMPLE_NAME);
        }

        [MenuItem("Assets/Create/LowPolyHnS/Developer/C# Complete Condition", false, 100)]
        public static void CreateCompleteConditionTemplate()
        {
            CreateGeneric(CONDITION_TEMPLATE_FILE_NORM, CONDITION_EXAMPLE_NAME);
        }

        [MenuItem("Assets/Create/LowPolyHnS/Developer/C# Igniter", false, 150)]
        public static void CreateIgniterTemplate()
        {
            CreateGeneric(IGNITER_TEMPLATE_FILE, IGNITER_EXAMPLE_NAME);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PRIVATE STATIC METHODS: ----------------------------------------------------------------

        private static void CreateGeneric(string templateName, string creationName)
        {
            string templatePath = Path.GetFullPath(Path.Combine(CORE_TEMPLATES_PATH, templateName));
            string creationPath = Path.Combine(GetSelectionPath(), creationName);
            CreateActionScript(templatePath, creationPath);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        public static string GetSelectionPath()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                break;
            }

            return path;
        }

        // SCRIPT CREATION: -----------------------------------------------------------------------

        private static void CreateActionScript(string templatePath, string creationPath)
        {
            string extension = Path.GetExtension(creationPath);
            string fileName = Path.GetFileName(creationPath);
            Texture2D icon = null;

            switch (extension)
            {
                case ".cs":
                    icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                    break;
                default:
                    icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
                    break;
            }

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<EndNameEdit_CreateActionScript>(),
                fileName,
                icon,
                templatePath
            );

            AssetDatabase.Refresh();
        }

        // FANCY STUFF: ---------------------------------------------------------------------------

        internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            string textContent = File.ReadAllText(resourceFile);

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);

            textContent = textContent.Replace(REPLACE_HEADER, "");
            textContent = textContent.Replace(REPLACE_FOOTER, "");

            string className = fileNameWithoutExtension.Replace(" ", "");
            textContent = textContent.Replace(REPLACE_CLASS_NAME, className);

            UTF8Encoding encoding = new UTF8Encoding(true);
            File.WriteAllText(fullPath, textContent, encoding);

            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
    }

    // INTERNAL CLASSES: --------------------------------------------------------------------------

    internal class EndNameEdit_CreateActionScript : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object asset = CreateCoreAsset.CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}