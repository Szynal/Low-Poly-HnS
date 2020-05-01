using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(LocalVariables))]
    public class LocalVariablesEditor : GenericVariablesEditor<MBVariableEditor, MBVariable>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private LocalVariables instance;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            instance = (LocalVariables) target;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            GlobalEditorID.Paint(instance);

            serializedObject.ApplyModifiedProperties();
        }

        protected override MBVariable[] GetReferences()
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

            MBVariable reference = instance.references[index];
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

        protected override MBVariable CreateReferenceInstance(string name)
        {
            MBVariable variable = instance.gameObject.AddComponent<MBVariable>();
            variable.variable.name = name;
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(variable.gameObject.scene);
            return variable;
        }

        protected override void DeleteReferenceInstance(int index)
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            MBVariable source = (MBVariable) spReferences
                .GetArrayElementAtIndex(index)
                .objectReferenceValue;

            spReferences.RemoveFromObjectArrayAt(index);
            RemoveSubEditorsElement(index);
            DestroyImmediate(source, true);

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();

            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
        }

        protected override Tag[] GetReferenceTags(int index)
        {
            return new Tag[0];
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/Variables/Local Variables", false, 0)]
        public static void CreateLocalVariables()
        {
            GameObject instance = CreateSceneObject.Create("Local Variables");
            instance.AddComponent<LocalVariables>();
        }
    }
}