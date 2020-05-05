using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    public abstract class GenericVariablesEditor<TEditor, TTarget> : MultiSubEditor<TEditor, TTarget>
        where TEditor : Editor
        where TTarget : Object
    {
        private class ItemReturnOperation
        {
            public bool removeIndex;
        }

        private const float TAG_PADDING = 1f;
        private const float TAG_WIDTH = 60f;

        private const string PROP_REFERENCES = "references";

        private const string CREATEVAR_PLACEHOLDER = "(New Variable Name)";
        private const string CREATEVAR_CONTROL_ID = "create-variable-text";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool enableEditor;
        private GUIStyle styleCreateVarText;
        private GUIStyle styleCreateVarButton;
        private GUIStyle styleCreateVarPlaceholder;

        private EditorSortableList editorSortableList;
        protected SerializedProperty spReferences;

        protected string search = "";
        protected int tagsMask = ~0;
        private string createVarInputText = "";

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            enableEditor = true;
            forceInitialize = true;
        }

        protected virtual void OnDisable()
        {
            CleanSubEditors();
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract TTarget[] GetReferences();
        protected abstract string GetReferenceName(int index);
        protected abstract Variable.DataType GetReferenceType(int index);
        protected abstract bool MatchSearch(int index, string search, int tagsMask);
        protected abstract TTarget CreateReferenceInstance(string name);
        protected abstract void DeleteReferenceInstance(int index);
        protected abstract Tag[] GetReferenceTags(int index);

        protected virtual void BeforePaintSubEditor(int index)
        {
        }

        protected virtual void AfterPaintSubEditorsList()
        {
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public void OnInspectorGUI(string search, int tagsMask)
        {
            this.search = search;
            this.tagsMask = tagsMask;
            bool usingSearch = !string.IsNullOrEmpty(this.search);
            PaintInspector(usingSearch);
        }

        public override void OnInspectorGUI()
        {
            PaintInspector(false);
        }

        private void PaintInspector(bool usingSearch)
        {
            if (enableEditor)
            {
                enableEditor = false;
                spReferences = serializedObject.FindProperty("references");

                UpdateSubEditors(GetReferences());
                editorSortableList = new EditorSortableList();
                InitializeStyles();
            }

            serializedObject.Update();
            UpdateSubEditors(GetReferences());

            PaintCreateVariable(usingSearch);
            PaintVariablesList(usingSearch);
            AfterPaintSubEditorsList();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void PaintCreateVariable(bool usingSearch)
        {
            if (usingSearch) return;

            EditorGUILayout.Space();
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );
            Rect rectText = new Rect(rect.x, rect.y, rect.width - 25f, rect.height);
            Rect rectCreate = new Rect(
                rectText.x + rectText.width,
                rectText.y,
                25f,
                rectText.height
            );

            GUI.SetNextControlName(CREATEVAR_CONTROL_ID);
            string textResult = EditorGUI.TextField(
                rectText,
                string.IsNullOrEmpty(createVarInputText) ? CREATEVAR_PLACEHOLDER : createVarInputText,
                string.IsNullOrEmpty(createVarInputText) ? styleCreateVarPlaceholder : styleCreateVarText
            );

            if (textResult == CREATEVAR_PLACEHOLDER) createVarInputText = "";
            else createVarInputText = textResult;

            bool pressEnter = Event.current.isKey &&
                              Event.current.keyCode == KeyCode.Return &&
                              GUI.GetNameOfFocusedControl() == CREATEVAR_CONTROL_ID;

            if (GUI.Button(rectCreate, "+", styleCreateVarButton) || pressEnter)
            {
                CreateVariable(createVarInputText);
                Event.current.Use();
            }
        }

        private void PaintVariablesList(bool usingSearch)
        {
            int removeReferenceIndex = -1;
            bool forceRepaint = false;

            int spReferencesSize = spReferences.arraySize;
            for (int i = 0; i < spReferencesSize; ++i)
            {
                if (usingSearch)
                {
                    if (!MatchSearch(i, search, tagsMask)) continue;
                }
                else
                {
                    bool forceSortRepaint = editorSortableList.CaptureSortEvents(handleRect[i], i);
                    forceRepaint = forceSortRepaint || forceRepaint;
                }

                EditorGUILayout.BeginVertical();
                ItemReturnOperation returnOperation = PaintReferenceHeader(i, usingSearch);
                if (returnOperation.removeIndex) removeReferenceIndex = i;

                using (var group = new EditorGUILayout.FadeGroupScope(isExpanded[i].faded))
                {
                    if (group.visible)
                    {
                        EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                        if (subEditors[i] != null)
                        {
                            BeforePaintSubEditor(i);
                            subEditors[i].OnInspectorGUI();
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndVertical();

                if (Event.current.type == EventType.Repaint)
                {
                    objectRect[i] = GUILayoutUtility.GetLastRect();
                }

                editorSortableList.PaintDropPoints(objectRect[i], i, spReferencesSize);
            }

            EditorGUILayout.Space();

            if (removeReferenceIndex >= 0)
            {
                DeleteReferenceInstance(removeReferenceIndex);
            }

            EditorSortableList.SwapIndexes swapIndexes = editorSortableList.GetSortIndexes();
            if (swapIndexes != null)
            {
                spReferences.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
                MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
            }

            if (EditorApplication.isPlaying) forceRepaint = true;
            if (forceRepaint) Repaint();
        }

        private ItemReturnOperation PaintReferenceHeader(int i, bool usingSearch)
        {
            ItemReturnOperation returnOperation = new ItemReturnOperation();

            Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            if (!usingSearch) PaintDragHandle(i, rectHeader);

            string variableName = isExpanded[i].target ? " ▾ " : " ▸ ";
            variableName += GetReferenceName(i);

            Texture2D variableIcon = VariableEditorUtils.GetIcon(GetReferenceType(i));

            GUIStyle style = isExpanded[i].target
                ? CoreGUIStyles.GetToggleButtonMidOn()
                : CoreGUIStyles.GetToggleButtonMidOff();

            Rect rectDelete = new Rect(
                rectHeader.x + rectHeader.width - 25f,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            Rect rectMain = new Rect(
                rectHeader.x + 25f,
                rectHeader.y,
                rectHeader.width - 25f * 2f,
                rectHeader.height
            );

            if (usingSearch)
            {
                style = isExpanded[i].target
                    ? CoreGUIStyles.GetToggleButtonLeftOn()
                    : CoreGUIStyles.GetToggleButtonLeftOff();

                rectMain = new Rect(
                    rectHeader.x,
                    rectHeader.y,
                    rectHeader.width - 25f,
                    rectHeader.height
                );
            }

            if (GUI.Button(rectMain, new GUIContent(variableName, variableIcon), style))
            {
                ToggleExpand(i);
            }

            GUIContent gcDelete = ClausesUtilities.Get(ClausesUtilities.Icon.Delete);
            if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
            {
                returnOperation.removeIndex = true;
            }

            PaintTags(i);
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

        private void PaintTags(int index)
        {
            Rect rectSpace = objectRect[index];
            rectSpace = new Rect(
                rectSpace.x,
                rectSpace.y + TAG_PADDING,
                rectSpace.width - 25f,
                20f - TAG_PADDING * 2f
            );

            Tag[] tags = GetReferenceTags(index);
            for (int i = 0; i < tags.Length; ++i)
            {
                if (string.IsNullOrEmpty(tags[i].name)) continue;

                Rect rect = new Rect(
                    rectSpace.x + rectSpace.width - TAG_WIDTH,
                    rectSpace.y,
                    TAG_WIDTH,
                    rectSpace.height
                );

                rectSpace = new Rect(
                    rectSpace.x,
                    rectSpace.y,
                    rectSpace.width - rect.width,
                    rectSpace.height
                );

                Color tempColor = GUI.backgroundColor;
                GUI.backgroundColor = tags[i].GetColor();
                EditorGUI.LabelField(rect, tags[i].name, CoreGUIStyles.GetLabelTag());
                GUI.backgroundColor = tempColor;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            float height = 18f;
            RectOffset margin = new RectOffset(0, 0, 2, 2);
            RectOffset padding = new RectOffset(5, 5, 0, 0);

            styleCreateVarText = new GUIStyle(EditorStyles.textField);
            styleCreateVarText.alignment = TextAnchor.MiddleRight;
            styleCreateVarText.margin = margin;
            styleCreateVarText.padding = padding;
            styleCreateVarText.fixedHeight = height;

            styleCreateVarPlaceholder = new GUIStyle(styleCreateVarText);
            styleCreateVarPlaceholder.fontStyle = FontStyle.Italic;
            styleCreateVarPlaceholder.normal.textColor = new Color(
                styleCreateVarPlaceholder.normal.textColor.r,
                styleCreateVarPlaceholder.normal.textColor.g,
                styleCreateVarPlaceholder.normal.textColor.b,
                0.5f
            );

            styleCreateVarButton = new GUIStyle(CoreGUIStyles.GetButtonRight());
            styleCreateVarButton.margin = margin;
            styleCreateVarButton.padding = new RectOffset(0, 0, 0, 0);
            styleCreateVarButton.fixedHeight = height;
        }

        protected TTarget CreateVariable(string variableName)
        {
            variableName = VariableEditor.ProcessName(variableName);

            EditorGUI.FocusTextInControl(null);
            GUIUtility.keyboardControl = 0;

            TTarget instance = CreateReferenceInstance(variableName);
            spReferences.AddToObjectArray(instance);
            AddSubEditorElement(instance, -1, true);

            return instance;
        }
    }
}