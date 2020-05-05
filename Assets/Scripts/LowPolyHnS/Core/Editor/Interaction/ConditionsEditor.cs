using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(Conditions))]
    public class ConditionsEditor : MultiSubEditor<ClauseEditor, Clause>
    {
        private static Clause CLIPBOARD_CLAUSE;

        private const string MSG_EMTPY_CONDITIONS = "There are no Clauses associated with this Conditions component.";
        private const string MSG_REMOVE_TITLE = "Are you sure you want to delete this Conditions Group?";
        private const string MSG_REMOVE_DESCR = "All information associated with this Conditions Group will be lost.";

        private const string MSG_PREFAB_UNSUPPORTED =
            "<b>LowPolyHnS</b> does not support creating <i>Prefabs</i> from <b>Conditions</b>";

        private const string PROP_INSTANCEID = "instanceID";
        private const string PROP_CLAUSES = "clauses";
        private const string PROP_DEFREAC = "defaultActions";

        private Conditions instance;
        private SerializedProperty spClauses;
        private SerializedProperty spDefaultActions;

        private ActionsEditor actionsEditor;
        private EditorSortableList editorSortableList;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            forceInitialize = true;

            instance = (Conditions) target;
            spClauses = serializedObject.FindProperty(PROP_CLAUSES);
            spDefaultActions = serializedObject.FindProperty(PROP_DEFREAC);

            if (instance.defaultActions != null)
            {
                actionsEditor = CreateEditor(instance.defaultActions) as ActionsEditor;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();

            UpdateSubEditors(instance.clauses);
            editorSortableList = new EditorSortableList();
        }

        protected override void Setup(ClauseEditor editor, int editorIndex)
        {
            editor.spClauses = spClauses;
            editor.parentConditions = instance;
        }

        // INSPECTOR: -----------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;

            serializedObject.Update();
            UpdateSubEditors(instance.clauses);

            PaintConditions();

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintConditions()
        {
            if (spClauses != null && spClauses.arraySize > 0)
            {
                PaintClauses();
            }
            else
            {
                EditorGUILayout.HelpBox(MSG_EMTPY_CONDITIONS, MessageType.None);
            }

            float widthAddClause = 100f;
            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddClause = new Rect(
                rectControls.x + rectControls.width / 2.0f - (widthAddClause + 25f) / 2.0f,
                rectControls.y,
                widthAddClause,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddClause.x + rectAddClause.width,
                rectControls.y,
                25f,
                rectControls.height
            );

            if (GUI.Button(rectAddClause, "Add Clause", CoreGUIStyles.GetButtonLeft()))
            {
                Clause clauseCreated = instance.gameObject.AddComponent<Clause>();

                int clauseCreatedIndex = spClauses.arraySize;
                spClauses.InsertArrayElementAtIndex(clauseCreatedIndex);
                spClauses.GetArrayElementAtIndex(clauseCreatedIndex).objectReferenceValue = clauseCreated;

                AddSubEditorElement(clauseCreated, -1, true);

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            GUIContent gcPaste = ClausesUtilities.Get(ClausesUtilities.Icon.Paste);
            EditorGUI.BeginDisabledGroup(CLIPBOARD_CLAUSE == null);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonRight()))
            {
                Clause copy = instance.gameObject.AddComponent<Clause>();
                EditorUtility.CopySerialized(CLIPBOARD_CLAUSE, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }

                int clauseIndex = spClauses.arraySize;
                spClauses.InsertArrayElementAtIndex(clauseIndex);
                spClauses.GetArrayElementAtIndex(clauseIndex).objectReferenceValue = copy;

                AddSubEditorElement(copy, -1, true);

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
                DestroyImmediate(CLIPBOARD_CLAUSE.gameObject, true);
                CLIPBOARD_CLAUSE = null;
            }

            EditorGUI.EndDisabledGroup();

            GUIContent gcElse = ClausesUtilities.Get(ClausesUtilities.Icon.Else);
            Rect rectElse = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectElse, gcElse, EditorStyles.boldLabel);

            ActionsEditor.Return returnActions = ActionsEditor.PaintActionsGUI(
                instance.gameObject,
                spDefaultActions,
                actionsEditor
            );

            if (returnActions != null)
            {
                spDefaultActions = returnActions.spParentActions;
                actionsEditor = returnActions.parentActionsEditor;

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            EditorGUILayout.Space();
        }

        private void PaintClauses()
        {
            int removeClauseIndex = -1;
            int duplicateClauseIndex = -1;
            int copyClauseIndex = -1;

            bool forceRepaint = false;
            int clauseSize = spClauses.arraySize;

            for (int i = 0; i < clauseSize; ++i)
            {
                if (subEditors == null || i >= subEditors.Length || subEditors[i] == null) continue;

                bool repaint = editorSortableList.CaptureSortEvents(subEditors[i].handleDragRect, i);
                forceRepaint = repaint || forceRepaint;

                EditorGUILayout.BeginVertical();
                Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
                PaintDragHandle(i, rectHeader);

                EditorGUIUtility.AddCursorRect(subEditors[i].handleDragRect, MouseCursor.Pan);
                string name = (isExpanded[i].target ? "▾ " : "▸ ") + instance.clauses[i].description;
                GUIStyle style = isExpanded[i].target
                    ? CoreGUIStyles.GetToggleButtonMidOn()
                    : CoreGUIStyles.GetToggleButtonMidOff();

                Rect rectDelete = new Rect(
                    rectHeader.x + rectHeader.width - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectDuplicate = new Rect(
                    rectDelete.x - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectCopy = new Rect(
                    rectDuplicate.x - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectMain = new Rect(
                    rectHeader.x + 25f,
                    rectHeader.y,
                    rectHeader.width - 25f * 4f,
                    rectHeader.height
                );

                if (GUI.Button(rectMain, name, style))
                {
                    ToggleExpand(i);
                }

                GUIContent gcCopy = ClausesUtilities.Get(ClausesUtilities.Icon.Copy);
                GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
                GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

                if (GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
                {
                    copyClauseIndex = i;
                }

                if (GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
                {
                    duplicateClauseIndex = i;
                }

                if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
                {
                    if (EditorUtility.DisplayDialog(MSG_REMOVE_TITLE, MSG_REMOVE_DESCR, "Yes", "Cancel"))
                    {
                        removeClauseIndex = i;
                    }
                }

                using (var group = new EditorGUILayout.FadeGroupScope(isExpanded[i].faded))
                {
                    if (group.visible)
                    {
                        EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        subEditors[i].OnClauseGUI();
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndVertical();
                if (Event.current.type == EventType.Repaint)
                {
                    subEditors[i].clauseRect = GUILayoutUtility.GetLastRect();
                }

                editorSortableList.PaintDropPoints(subEditors[i].clauseRect, i, clauseSize);
            }

            if (copyClauseIndex >= 0)
            {
                Clause source = (Clause) subEditors[copyClauseIndex].target;
                GameObject copyInstance = EditorUtility.CreateGameObjectWithHideFlags(
                    "Clause (Copy)",
                    HideFlags.HideAndDontSave
                );

                CLIPBOARD_CLAUSE = (Clause) copyInstance.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_CLAUSE);

                if (CLIPBOARD_CLAUSE.conditionsList != null)
                {
                    IConditionsList conditionsListSource = CLIPBOARD_CLAUSE.conditionsList;
                    IConditionsList conditionsListCopy = instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(CLIPBOARD_CLAUSE);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }
            }

            if (duplicateClauseIndex >= 0)
            {
                int srcIndex = duplicateClauseIndex;
                int dstIndex = duplicateClauseIndex + 1;

                Clause source = (Clause) subEditors[srcIndex].target;
                Clause copy = (Clause) instance.gameObject.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(ClauseEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;

                    if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
                    soCopy.ApplyModifiedPropertiesWithoutUndo();
                    soCopy.Update();
                }

                spClauses.InsertArrayElementAtIndex(dstIndex);
                spClauses.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                spClauses.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                spClauses.serializedObject.Update();

                AddSubEditorElement(copy, dstIndex, true);
            }

            if (removeClauseIndex >= 0)
            {
                subEditors[removeClauseIndex].OnDestroyClause();
                Clause rmClause = (Clause) spClauses
                    .GetArrayElementAtIndex(removeClauseIndex).objectReferenceValue;

                spClauses.DeleteArrayElementAtIndex(removeClauseIndex);
                spClauses.RemoveFromObjectArrayAt(removeClauseIndex);

                DestroyImmediate(rmClause, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
            }

            EditorSortableList.SwapIndexes swapIndexes = editorSortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                spClauses.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
                MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
            }

            if (forceRepaint) Repaint();
        }

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect rect = new Rect(
                rectHeader.x,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            GUI.Label(rect, "=", CoreGUIStyles.GetButtonLeft());
            if (Event.current.type == EventType.Repaint)
            {
                subEditors[i].handleDragRect = rect;
            }
        }

        public static void DuplicateIConditionList(IConditionsList source, IConditionsList dest)
        {
            if (source == null || source.conditions == null || source.conditions.Length == 0) return;
            ICondition[] conditions = new ICondition[source.conditions.Length];

            for (int i = 0; i < source.conditions.Length; i++)
            {
                ICondition sourceAction = source.conditions[i];
                if (sourceAction == null) continue;
                conditions[i] = dest.gameObject.AddComponent(sourceAction.GetType()) as ICondition;
                EditorUtility.CopySerialized(sourceAction, conditions[i]);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(source.gameObject.scene);
            }

            dest.conditions = conditions;
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/Conditions", false, 0)]
        public static void CreateConditions()
        {
            GameObject conditionsAsset = CreateSceneObject.Create("Conditions");
            conditionsAsset.AddComponent<Conditions>();
        }
    }
}