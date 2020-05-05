using System;
using System.Reflection;
using System.Text.RegularExpressions;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [CustomEditor(typeof(MBVariable))]
    public class MBVariableEditor : VariableEditor
    {
        protected override bool UseTags()
        {
            return false;
        }

        protected override bool CanSave()
        {
            return ((MBVariable) target).CanSave();
        }

        public override Variable GetRuntimeVariable()
        {
            return LocalVariablesUtilities.Get(
                ((MBVariable) target).gameObject,
                spVariableName.stringValue,
                false
            );
        }
    }

    [CustomEditor(typeof(SOVariable))]
    public class SOVariableEditor : VariableEditor
    {
        protected override bool UseTags()
        {
            return true;
        }

        protected override bool CanSave()
        {
            return ((SOVariable) target).CanSave();
        }

        public override Variable GetRuntimeVariable()
        {
            return GlobalVariablesUtilities.Get(
                spVariableName.stringValue
            );
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////

    public abstract class VariableEditor : Editor
    {
        private static readonly GUIContent GUICONTENT_NAME = new GUIContent("Name");
        private static readonly GUIContent GUICONTENT_TYPE = new GUIContent("Type");
        private static readonly GUIContent GUICONTENT_TAGS = new GUIContent("Tags");
        private static readonly GUIContent GUICONTENT_VALUE = new GUIContent("Value");

        private static readonly Regex REGEX_VARNAME = new Regex(@"[^\p{L}\p{Nd}-_]");
        private static readonly Regex REGEX_VARPATH = new Regex(@"[^\p{L}\p{Nd}-_\/]");

        private const string PROP_VARIABLE = "variable";
        private const string PROP_NAME = "name";
        private const string PROP_SAVE = "save";
        private const string PROP_TYPE = "type";
        private const string PROP_TAGS = "tags";

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spVariable;
        public bool editableType = true;
        public bool editableCommon = true;

        public SerializedProperty spVariableName;
        public SerializedProperty spVariableSave;
        public SerializedProperty spVariableType;
        public SerializedProperty spVariableTags;

        public SerializedProperty spVariableStr;
        public SerializedProperty spVariableInt;
        public SerializedProperty spVariableNum;
        public SerializedProperty spVariableBol;
        public SerializedProperty spVariableCol;
        public SerializedProperty spVariableVc2;
        public SerializedProperty spVariableVc3;
        public SerializedProperty spVariableTxt;
        public SerializedProperty spVariableSpr;
        public SerializedProperty spVariableObj;
        public SerializedProperty spVariableTrn;
        public SerializedProperty spVariableRbd;

        private SerializedProperty spRuntimeValue;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            target.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            spVariable = serializedObject.FindProperty(PROP_VARIABLE);

            spVariableName = spVariable.FindPropertyRelative(PROP_NAME);
            spVariableSave = spVariable.FindPropertyRelative(PROP_SAVE);
            spVariableType = spVariable.FindPropertyRelative(PROP_TYPE);
            spVariableTags = spVariable.FindPropertyRelative(PROP_TAGS);

            spVariableStr = spVariable.FindPropertyRelative("varStr");
            spVariableInt = spVariable.FindPropertyRelative("varInt");
            spVariableNum = spVariable.FindPropertyRelative("varNum");
            spVariableBol = spVariable.FindPropertyRelative("varBol");
            spVariableCol = spVariable.FindPropertyRelative("varCol");
            spVariableVc2 = spVariable.FindPropertyRelative("varVc2");
            spVariableVc3 = spVariable.FindPropertyRelative("varVc3");
            spVariableTxt = spVariable.FindPropertyRelative("varTxt");
            spVariableSpr = spVariable.FindPropertyRelative("varSpr");
            spVariableObj = spVariable.FindPropertyRelative("varObj");
            spVariableTrn = spVariable.FindPropertyRelative("varTrn");
            spVariableRbd = spVariable.FindPropertyRelative("varRbd");
        }

        // PUBLIC METHODDS: -----------------------------------------------------------------------

        public string GetName()
        {
            if (spVariableSave.boolValue) return spVariableName.stringValue + " (save)";
            return spVariableName.stringValue;
        }

        public bool MatchSearch(string search, int tagsMask)
        {
            bool matchSearch = spVariableName.stringValue.Contains(search);
            bool matchTags = (tagsMask & spVariableTags.intValue) != 0;
            return matchSearch && matchTags;
        }

        public static string ProcessName(string name, bool isPath = false)
        {
            string processed = name.Trim();
            switch (isPath)
            {
                case true:
                    processed = REGEX_VARPATH.Replace(processed, "-");
                    break;
                case false:
                    processed = REGEX_VARNAME.Replace(processed, "-");
                    break;
            }

            return processed;
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected abstract bool UseTags();
        protected abstract bool CanSave();
        public abstract Variable GetRuntimeVariable();

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical();

            PaintCommon();
            PaintType();

            if (EditorApplication.isPlaying) PaintRuntimeValue();
            else PaintValue();

            PaintTags();

            EditorGUILayout.EndVertical();
            GUILayout.Space(2f);
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private void PaintCommon()
        {
            if (!editableCommon) return;

            float saveWidth = 18f;
            float saveOffset = 5f;
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectName = new Rect(
                rect.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width - saveWidth - saveOffset,
                rect.height
            );
            Rect rectSave = new Rect(
                rectName.x + rectName.width + saveOffset,
                rect.y,
                saveWidth,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_NAME);
            string previousName = spVariableName.stringValue;

            EditorGUI.PropertyField(rectName, spVariableName, GUIContent.none);

            if (!CanSave() && spVariableSave.boolValue)
            {
                spVariableSave.boolValue = false;
            }

            EditorGUI.BeginDisabledGroup(!CanSave());
            EditorGUI.PropertyField(rectSave, spVariableSave, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(2f);

            if (previousName != spVariableName.stringValue)
            {
                string varName = ProcessName(spVariableName.stringValue);
                spVariableName.stringValue = varName;
            }
        }


        private void PaintType()
        {
            if (!editableType) return;

            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectDropdown = new Rect(
                rect.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_TYPE);

            string typeName = ((Variable.DataType) spVariableType.intValue).ToString();
            if (EditorGUI.DropdownButton(rectDropdown, new GUIContent(typeName), FocusType.Keyboard))
            {
                SelectTypePanel selectTypePanel = new SelectTypePanel(
                    ChangeTypeCallback,
                    "Variables",
                    typeof(VariableBase),
                    rectDropdown.width
                );

                PopupWindow.Show(rectDropdown, selectTypePanel);
            }

            GUILayout.Space(2f);
        }

        private void PaintValue()
        {
            switch ((Variable.DataType) spVariableType.intValue)
            {
                case Variable.DataType.String:
                    PaintProperty(spVariableStr);
                    break;
                case Variable.DataType.Number:
                    PaintProperty(spVariableNum);
                    break;
                case Variable.DataType.Bool:
                    PaintProperty(spVariableBol);
                    break;
                case Variable.DataType.Color:
                    PaintProperty(spVariableCol);
                    break;
                case Variable.DataType.Vector2:
                    PaintProperty(spVariableVc2);
                    break;
                case Variable.DataType.Vector3:
                    PaintProperty(spVariableVc3);
                    break;
                case Variable.DataType.Texture2D:
                    PaintProperty(spVariableTxt);
                    break;
                case Variable.DataType.Sprite:
                    PaintProperty(spVariableSpr);
                    break;
                case Variable.DataType.GameObject:
                    PaintProperty(spVariableObj);
                    break;
            }
        }

        private void PaintTags()
        {
            if (!UseTags()) return;
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectMask = new Rect(
                rect.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width,
                rect.height
            );


            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_TAGS);
            spVariableTags.intValue = EditorGUI.MaskField(
                rectMask,
                spVariableTags.intValue,
                GlobalTagsEditor.GetTagNames()
            );
        }

        public void PaintRuntimeValue()
        {
            Variable runtime = GetRuntimeVariable();
            object variable = runtime == null ? null : runtime.Get();

            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.LabelField(
                rect,
                "Runtime Value",
                variable == null ? "(null)" : variable.ToString(),
                EditorStyles.boldLabel
            );
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ChangeTypeCallback(Type variableType)
        {
            MethodInfo methodInfo = variableType.GetMethod("GetDataType");
            if (methodInfo != null)
            {
                spVariableType.intValue = (int) methodInfo.Invoke(null, null);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void PaintProperty(SerializedProperty property)
        {
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth,
                EditorGUI.GetPropertyHeight(property)
            );
            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                EditorGUIUtility.labelWidth,
                rect.height
            );
            Rect rectField = new Rect(
                rectLabel.x + rectLabel.width,
                rect.y,
                rect.width - rectLabel.width,
                rect.height
            );

            EditorGUI.PrefixLabel(rectLabel, GUICONTENT_VALUE);
            EditorGUI.PropertyField(rectField, property, GUIContent.none);
            GUILayout.Space(2f);
        }
    }
}