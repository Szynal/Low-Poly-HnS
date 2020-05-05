using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(IConditionsList), true)]
    public class IConditionsListEditor : MultiSubEditor<IConditionEditor, ICondition>
    {
        private class ItemReturnOperation
        {
            public bool removeIndex;
            public bool duplicateIndex;
            public bool copyIndex;
        }

        private static ICondition CLIPBOARD_ICONDITION;

        private const string MSG_OVERWRITE_TITLE = "There's already a Conditions component.";

        private const string MSG_OVERWRITE_DESCR =
            "Do you want to replace the previous Conditions game object with an empty one?";

        private const string MSG_EMPTY_CONDITIONS =
            "There is no Conditions attached. Add an existing component or create a new one.";

        private const string MSG_UNDEF_CONDITION_1 = "This Conditions is not set as an instance of an object.";
        private const string MSG_UNDEF_CONDITION_2 = "Check if you disabled or uninstalled a module that defined it.";

        public const string PROP_CONDITIONS = "conditions";

        public IConditionsList instance;
        public SerializedProperty spConditions;

        private Rect addConditionsButtonRect = Rect.zero;
        private EditorSortableList editorSortableList;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            forceInitialize = true;
            instance = (IConditionsList) target;
            spConditions = serializedObject.FindProperty(PROP_CONDITIONS);

            UpdateSubEditors(instance.conditions);
            editorSortableList = new EditorSortableList();

            if (target != null) target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnDisable()
        {
            CleanSubEditors();
            editorSortableList = null;
        }

        protected override void Setup(IConditionEditor editor, int editorIndex)
        {
            editor.spConditions = spConditions;
            editor.Setup(spConditions, editorIndex);
        }

        public void OnDestroyConditionsList()
        {
            for (int i = 0; i < instance.conditions.Length; ++i)
            {
                DestroyImmediate(instance.conditions[i]);
            }
        }

        // INSPECTOR: -----------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;
            serializedObject.Update();
            UpdateSubEditors(instance.conditions);

            int removConditionIndex = -1;
            int duplicateConditionIndex = -1;
            int copyConditionIndex = -1;
            bool forceRepaint = false;
            bool conditionsCollapsed = true;

            int spConditionsSize = spConditions.arraySize;
            for (int i = 0; i < spConditionsSize; ++i)
            {
                bool forceSortRepaint = editorSortableList.CaptureSortEvents(handleRect[i], i);
                forceRepaint = forceSortRepaint || forceRepaint;

                GUILayout.BeginVertical();
                ItemReturnOperation returnOperation = PaintConditionsHeader(i);
                if (returnOperation.removeIndex) removConditionIndex = i;
                if (returnOperation.duplicateIndex) duplicateConditionIndex = i;
                if (returnOperation.copyIndex) copyConditionIndex = i;

                conditionsCollapsed &= isExpanded[i].target;
                using (var group = new EditorGUILayout.FadeGroupScope(isExpanded[i].faded))
                {
                    if (group.visible)
                    {
                        EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        if (subEditors[i] != null) subEditors[i].OnInspectorGUI();
                        else
                        {
                            EditorGUILayout.HelpBox(MSG_UNDEF_CONDITION_1, MessageType.Warning);
                            EditorGUILayout.HelpBox(MSG_UNDEF_CONDITION_2, MessageType.None);
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                GUILayout.EndVertical();

                if (Event.current.type == EventType.Repaint)
                {
                    objectRect[i] = GUILayoutUtility.GetLastRect();
                }

                editorSortableList.PaintDropPoints(objectRect[i], i, spConditionsSize);
            }

            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddConditions = new Rect(
                rectControls.x,
                rectControls.y,
                SelectTypePanel.WINDOW_WIDTH,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddConditions.x + rectAddConditions.width,
                rectControls.y,
                25f,
                rectControls.height
            );

            Rect rectPlay = new Rect(
                rectControls.x + rectControls.width - 25f,
                rectControls.y,
                25f,
                rectControls.height
            );

            Rect rectCollapse = new Rect(
                rectPlay.x - 25f,
                rectPlay.y,
                25f,
                rectPlay.height
            );

            Rect rectUnused = new Rect(
                rectPaste.x + rectPaste.width,
                rectControls.y,
                rectControls.width - (rectPaste.x + rectPaste.width - rectControls.x) - rectPlay.width -
                rectCollapse.width,
                rectControls.height
            );

            if (GUI.Button(rectAddConditions, "Add Condition", CoreGUIStyles.GetToggleButtonLeftAdd()))
            {
                SelectTypePanel selectTypePanel =
                    new SelectTypePanel(AddNewCondition, "Conditions", typeof(ICondition));
                PopupWindow.Show(addConditionsButtonRect, selectTypePanel);
            }

            if (Event.current.type == EventType.Repaint)
            {
                addConditionsButtonRect = rectAddConditions;
            }

            GUIContent gcPaste = ClausesUtilities.Get(ClausesUtilities.Icon.Paste);
            EditorGUI.BeginDisabledGroup(CLIPBOARD_ICONDITION == null);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonMid()))
            {
                ICondition conditionCreated =
                    (ICondition) instance.gameObject.AddComponent(CLIPBOARD_ICONDITION.GetType());
                EditorUtility.CopySerialized(CLIPBOARD_ICONDITION, conditionCreated);

                spConditions.AddToObjectArray(conditionCreated);
                AddSubEditorElement(conditionCreated, -1, true);

                DestroyImmediate(CLIPBOARD_ICONDITION.gameObject, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
                CLIPBOARD_ICONDITION = null;
            }

            EditorGUI.EndDisabledGroup();

            GUI.Label(rectUnused, "", CoreGUIStyles.GetToggleButtonMidOn());

            GUIContent gcToggle = conditionsCollapsed
                ? ClausesUtilities.Get(ClausesUtilities.Icon.Collapse)
                : ClausesUtilities.Get(ClausesUtilities.Icon.Expand);

            EditorGUI.BeginDisabledGroup(instance.conditions.Length == 0);
            if (GUI.Button(rectCollapse, gcToggle, CoreGUIStyles.GetButtonMid()))
            {
                for (int i = 0; i < subEditors.Length; ++i)
                {
                    SetExpand(i, !conditionsCollapsed);
                }
            }

            EditorGUI.EndDisabledGroup();

            GUIContent gcPlay = ClausesUtilities.Get(ClausesUtilities.Icon.Play);
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            if (GUI.Button(rectPlay, gcPlay, CoreGUIStyles.GetButtonRight()))
            {
                Debug.LogFormat(
                    "<b>Conditions Evaluation:</b> {0}",
                    instance.Check(instance.gameObject)
                );
            }

            EditorGUI.EndDisabledGroup();

            if (removConditionIndex >= 0)
            {
                ICondition source =
                    (ICondition) spConditions.GetArrayElementAtIndex(removConditionIndex).objectReferenceValue;

                spConditions.RemoveFromObjectArrayAt(removConditionIndex);
                RemoveSubEditorsElement(removConditionIndex);
                DestroyImmediate(source, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
            }

            if (duplicateConditionIndex >= 0)
            {
                int srcIndex = duplicateConditionIndex;
                int dstIndex = duplicateConditionIndex + 1;

                ICondition source = (ICondition) subEditors[srcIndex].target;
                ICondition copy = (ICondition) instance.gameObject.AddComponent(source.GetType());

                spConditions.InsertArrayElementAtIndex(dstIndex);
                spConditions.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                EditorUtility.CopySerialized(source, copy);
                AddSubEditorElement(copy, dstIndex, true);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
            }

            if (copyConditionIndex >= 0)
            {
                ICondition source = (ICondition) subEditors[copyConditionIndex].target;

                GameObject conditionInstance = new GameObject();
                conditionInstance.hideFlags = HideFlags.HideAndDontSave;

                CLIPBOARD_ICONDITION = (ICondition) conditionInstance.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_ICONDITION);
            }

            EditorSortableList.SwapIndexes swapIndexes = editorSortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                spConditions.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
                MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
            }

            if (EditorApplication.isPlaying) forceRepaint = true;
            if (forceRepaint) Repaint();
            serializedObject.ApplyModifiedProperties();
        }

        private ItemReturnOperation PaintConditionsHeader(int i)
        {
            ItemReturnOperation returnOperation = new ItemReturnOperation();

            Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            PaintDragHandle(i, rectHeader);

            Texture2D conditionIcon = i < subEditors.Length && subEditors[i] != null
                ? subEditors[i].GetIcon()
                : null;

            string conditionName = isExpanded[i].target ? " ▾ " : " ▸ ";
            conditionName += instance.conditions[i] != null
                ? instance.conditions[i].GetNodeTitle()
                : "<i>Undefined Condition</i>";

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

            if (GUI.Button(rectMain, new GUIContent(conditionName, conditionIcon), style))
            {
                ToggleExpand(i);
            }

            GUIContent gcCopy = ClausesUtilities.Get(ClausesUtilities.Icon.Copy);
            GUIContent gcDuplicate = ClausesUtilities.Get(ClausesUtilities.Icon.Duplicate);
            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);

            if (instance.conditions[i] != null && GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
            {
                returnOperation.copyIndex = true;
            }

            if (instance.conditions[i] != null && GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
            {
                returnOperation.duplicateIndex = true;
            }

            if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
            {
                returnOperation.removeIndex = true;
            }

            return returnOperation;
        }

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect handle = new Rect(rectHeader.x, rectHeader.y, 25f, rectHeader.height);
            GUI.Label(handle, "=", CoreGUIStyles.GetButtonLeft());

            if (Event.current.type == EventType.Repaint)
            {
                handleRect[i] = handle;
            }

            EditorGUIUtility.AddCursorRect(handleRect[i], MouseCursor.Pan);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void AddNewCondition(Type conditionType)
        {
            ICondition conditionCreated = (ICondition) instance.gameObject.AddComponent(conditionType);

            spConditions.AddToObjectArray(conditionCreated);
            AddSubEditorElement(conditionCreated, -1, true);
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
        }
    }
}