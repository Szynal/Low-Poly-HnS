using System.Collections.Generic;
using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(GlobalVariables))]
    public class GlobalVariablesEditor : GenericVariablesEditor<SOVariableEditor, SOVariable>
    {
        private const string LABEL_TITLE = "Global Variables";

        public const string PATH_ASSET = "Assets/Scripts/Databases/Core/Variables/";
        public const string NAME_ASSET = "Variables.asset";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SearchField searchField;

        private Rect editTagsRect = Rect.zero;
        private GlobalVariables instance;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            searchField = new SearchField();
            instance = target as GlobalVariables;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            PaintSearch();

            EditorGUILayout.LabelField(LABEL_TITLE, EditorStyles.boldLabel);
            base.OnInspectorGUI(search, tagsMask);
        }

        private void PaintSearch()
        {
            EditorGUILayout.BeginHorizontal();

            search = searchField.OnGUI(search);
            GUILayoutOption[] options =
            {
                GUILayout.Width(80f),
                GUILayout.Height(18f)
            };

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(search));
            string[] tags = GlobalTagsEditor.GetTagNames();
            tagsMask = EditorGUILayout.MaskField(
                GUIContent.none,
                tagsMask,
                tags,
                EditorStyles.miniButtonLeft,
                options
            );
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Edit Tags", EditorStyles.miniButtonRight, options))
            {
                PopupWindow.Show(editTagsRect, new TagsEditorWindow());
            }

            if (Event.current.type == EventType.Repaint)
            {
                editTagsRect = GUILayoutUtility.GetLastRect();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override SOVariable[] GetReferences()
        {
            return instance.references;
        }

        protected override string GetReferenceName(int index)
        {
            if (index < 0 || index >= instance.references.Length)
            {
                return "<i>Unbound Variable</i>";
            }

            if (instance.references[index] == null)
            {
                return "<i>Undefined Variable</i>";
            }

            return subEditors[index].GetName();
        }

        protected override Variable.DataType GetReferenceType(int index)
        {
            if (index >= instance.references.Length) return Variable.DataType.Null;

            SOVariable reference = instance.references[index];
            if (reference == null) return Variable.DataType.Null;

            Variable variable = reference.variable;
            return (Variable.DataType) variable.type;
        }

        protected override bool MatchSearch(int index, string search, int tagsMask)
        {
            if (index >= subEditors.Length) return false;
            if (subEditors[index] == null) return false;
            return subEditors[index].MatchSearch(search, tagsMask);
        }

        protected override SOVariable CreateReferenceInstance(string name)
        {
            SOVariable variable = CreateInstance<SOVariable>();
            variable.name = LowPolyHnSUtilities.RandomHash(8);
            variable.variable.name = name;

            AssetDatabase.AddObjectToAsset(variable, instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(Path.Combine(PATH_ASSET, NAME_ASSET));

            return variable;
        }

        protected override void DeleteReferenceInstance(int index)
        {
            SOVariable source = (SOVariable) spReferences
                .GetArrayElementAtIndex(index)
                .objectReferenceValue;

            spReferences.RemoveFromObjectArrayAt(index);
            RemoveSubEditorsElement(index);

            DestroyImmediate(source, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(Path.Combine(PATH_ASSET, NAME_ASSET));
            Undo.ClearAll();
        }

        protected override Tag[] GetReferenceTags(int index)
        {
            List<Tag> result = new List<Tag>();

            if (index >= instance.references.Length) return result.ToArray();

            SOVariable reference = instance.references[index];
            if (reference == null) return result.ToArray();

            int mask = reference.variable.tags;
            Tag[] tags = GlobalTagsEditor.GetTags();

            for (int i = 0; i < tags.Length; ++i)
            {
                if ((mask & 1) != 0)
                {
                    result.Add(tags[i]);
                }

                mask >>= 1;
            }

            return result.ToArray();
        }
    }
}