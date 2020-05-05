using System;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(ListVariables))]
    public class ListVariablesEditor : LocalVariablesEditor
    {
        private const string RUNTIME_NAME = "{0}{1}";
        private const string REFERENCE_NAME = "Item: {0} {1}";
        private const string SAVE = "(save)";

        // PROPERTIES: ----------------------------------------------------------------------------

        private ListVariables list;

        private SerializedProperty spType;
        private SerializedProperty spSave;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();

            list = target as ListVariables;
            spType = serializedObject.FindProperty("type");
            spSave = serializedObject.FindProperty("save");
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) PaintRuntimeInspector();
            else base.OnInspectorGUI();
        }

        private void PaintRuntimeInspector()
        {
            EditorGUILayout.Space();

            if (list.variables.Count == 0)
            {
                EditorGUILayout.HelpBox("Empty List", MessageType.Info);
                return;
            }

            GUILayoutOption width = GUILayout.Width(150);
            GUILayoutOption height = GUILayout.Height(20);
            EditorGUI.BeginDisabledGroup(true);

            for (int i = 0; i < list.variables.Count; ++i)
            {
                object variable = list.variables[i].Get();
                string title = string.Format(
                    RUNTIME_NAME,
                    list.iterator == i ? " ▸ " : string.Empty,
                    string.Format(REFERENCE_NAME, i, string.Empty)
                );

                string value = variable == null
                    ? "(null)"
                    : variable.ToString();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Button(title, CoreGUIStyles.GetToggleButtonLeftOff(), height, width);
                GUILayout.Button(value, CoreGUIStyles.GetToggleButtonRightOn(), height);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            serializedObject.Update();
            GlobalEditorID.Paint(list);
            serializedObject.ApplyModifiedProperties();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void PaintCreateVariable(bool usingSearch)
        {
            if (usingSearch) return;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spType, GUIContent.none, GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < subEditors.Length; ++i)
                {
                    subEditors[i].serializedObject.ApplyModifiedProperties();
                    subEditors[i].serializedObject.Update();

                    subEditors[i].spVariableType.intValue = spType.intValue;

                    subEditors[i].serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    subEditors[i].serializedObject.Update();
                }
            }

            if (!CanSave() && spSave.boolValue) spSave.boolValue = false;
            EditorGUI.BeginDisabledGroup(!CanSave());

            spSave.boolValue = EditorGUILayout.ToggleLeft(
                spSave.displayName,
                spSave.boolValue
            );

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        protected override string GetReferenceName(int index)
        {
            return string.Format(
                REFERENCE_NAME,
                index,
                spSave.boolValue ? SAVE : string.Empty
            );
        }

        protected override void BeforePaintSubEditor(int index)
        {
            subEditors[index].editableType = false;
            subEditors[index].editableCommon = false;
        }

        protected override void AfterPaintSubEditorsList()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Height(20), GUILayout.Width(100)))
            {
                string variableName = Guid.NewGuid().ToString("N");
                MBVariable variable = CreateVariable(variableName);

                variable.variable.type = spType.intValue;
                Event.current.Use();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private bool CanSave()
        {
            Variable.DataType type = (Variable.DataType) spType.enumValueIndex;
            return Variable.CanSave(type);
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/Variables/List Variables", false, 0)]
        public static void CreateListVariables()
        {
            GameObject instance = CreateSceneObject.Create("List Variables");
            instance.AddComponent<ListVariables>();
        }
    }
}