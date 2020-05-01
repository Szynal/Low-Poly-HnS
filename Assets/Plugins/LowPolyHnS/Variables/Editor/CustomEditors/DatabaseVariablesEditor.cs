using System.IO;
using LowPolyHnS.Core;
using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(DatabaseVariables))]
    public class DatabaseVariablesEditor : IDatabaseEditor
    {
        private const string PROP_GLOBALTAGS = "tags";
        private const string PROP_GLOBALVARIABLES = "variables";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTags;
        private SerializedProperty spVariables;
        private GlobalVariablesEditor variablesEditor;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            spTags = serializedObject.FindProperty(PROP_GLOBALTAGS);
            if (spTags.objectReferenceValue == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(GlobalTagsEditor.PATH_ASSET);
                GlobalTags instance = CreateInstance<GlobalTags>();
                AssetDatabase.CreateAsset(instance, Path.Combine(
                    GlobalTagsEditor.PATH_ASSET,
                    GlobalTagsEditor.NAME_ASSET
                ));

                spTags.objectReferenceValue = instance;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            spVariables = serializedObject.FindProperty(PROP_GLOBALVARIABLES);
            if (spVariables.objectReferenceValue == null)
            {
                LowPolyHnSUtilities.CreateFolderStructure(GlobalVariablesEditor.PATH_ASSET);
                GlobalVariables instance = CreateInstance<GlobalVariables>();
                AssetDatabase.CreateAsset(instance, Path.Combine(
                    GlobalVariablesEditor.PATH_ASSET,
                    GlobalVariablesEditor.NAME_ASSET
                ));

                spVariables.objectReferenceValue = instance;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            variablesEditor = (GlobalVariablesEditor) CreateEditor(
                spVariables.objectReferenceValue
            );
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        public override string GetName()
        {
            return "Variables";
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (variablesEditor != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                variablesEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}