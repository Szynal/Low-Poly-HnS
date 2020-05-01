using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LowPolyHnS.Core
{
    public abstract class MultiSubEditor<TEditor, TTarget> : Editor
        where TEditor : Editor
        where TTarget : Object
    {
        private const string PROP_IS_EXPANDED = "isExpanded";
        private const float ANIM_BOOL_SPEED = 3.0f;

        public TEditor[] subEditors { get; private set; }

        protected AnimBool[] isExpanded;
        protected SerializedProperty[] expandProp;

        protected Rect[] handleRect;
        protected Rect[] objectRect;

        public bool forceInitialize;

        public void UpdateSubEditors(TTarget[] subInstances)
        {
            if (serializedObject == null) return;

            if (!forceInitialize && subEditors != null &&
                subEditors.Length == subInstances.Length)
            {
                bool difference = false;
                for (int i = 0; i < subEditors.Length; ++i)
                {
                    if (subEditors[i] == null) continue;
                    if (subInstances[i] == null) continue;

                    if (subEditors[i].target == subInstances[i]) continue;
                    difference = true;
                    break;
                }

                if (!difference) return;
            }

            forceInitialize = false;
            CleanSubEditors();

            subEditors = new TEditor[subInstances.Length];
            isExpanded = new AnimBool[subInstances.Length];
            handleRect = new Rect[subInstances.Length];
            objectRect = new Rect[subInstances.Length];
            expandProp = new SerializedProperty[subInstances.Length];

            int length = subInstances.Length;
            for (int i = 0; i < length; i++)
            {
                if (subInstances[i] != null)
                {
                    subEditors[i] = CreateEditor(subInstances[i]) as TEditor;
                    Setup(subEditors[i], i);

                    expandProp[i] = subEditors[i]
                        .serializedObject
                        .FindProperty(PROP_IS_EXPANDED);
                }

                bool expand = expandProp[i] != null && expandProp[i].boolValue;

                handleRect[i] = Rect.zero;
                objectRect[i] = Rect.zero;
                isExpanded[i] = new AnimBool(expand) {speed = ANIM_BOOL_SPEED};
                isExpanded[i].valueChanged.AddListener(Repaint);
            }
        }

        public void CleanSubEditors()
        {
            if (subEditors == null) return;

            for (int i = 0; i < subEditors.Length; i++)
            {
                if (subEditors[i] == null) continue;
                DestroyImmediate(subEditors[i]);
            }

            subEditors = null;
            isExpanded = null;
            handleRect = null;
            objectRect = null;
            expandProp = null;
        }

        public void ToggleExpand(int index)
        {
            SetExpand(index, !isExpanded[index].target);
        }

        public void SetExpand(int index, bool state)
        {
            isExpanded[index].target = state;
            if (expandProp[index] != null)
            {
                subEditors[index].serializedObject.Update();
                expandProp[index].boolValue = state;

                subEditors[index].serializedObject.ApplyModifiedProperties();
                subEditors[index].serializedObject.Update();
            }
        }

        protected void AddSubEditorElement(TTarget target, int index, bool openItem)
        {
            List<TEditor> tmpSubEditor = new List<TEditor>(subEditors);
            List<AnimBool> tmpIsExpanded = new List<AnimBool>(isExpanded);
            List<Rect> tmpHandleRect = new List<Rect>(handleRect);
            List<Rect> tmpActionRect = new List<Rect>(objectRect);
            List<SerializedProperty> tmpExpandProp = new List<SerializedProperty>(expandProp);

            if (index < 0) index = subEditors.Length;

            if (index >= subEditors.Length) tmpSubEditor.Add(CreateEditor(target) as TEditor);
            else tmpSubEditor.Insert(index, CreateEditor(target) as TEditor);

            Setup(tmpSubEditor[index], index);


            AnimBool element = new AnimBool(false);
            element.target = openItem;
            element.speed = ANIM_BOOL_SPEED;
            element.valueChanged.AddListener(Repaint);

            if (index >= subEditors.Length) tmpHandleRect.Add(Rect.zero);
            else tmpHandleRect.Insert(index, Rect.zero);

            if (index >= subEditors.Length) tmpActionRect.Add(Rect.zero);
            else tmpActionRect.Insert(index, Rect.zero);

            if (index >= subEditors.Length) tmpIsExpanded.Add(element);
            else tmpIsExpanded.Insert(index, element);

            SerializedProperty expandProperty = tmpSubEditor[index].serializedObject.FindProperty(PROP_IS_EXPANDED);
            if (expandProperty != null)
            {
                tmpSubEditor[index].serializedObject.Update();
                expandProperty.boolValue = openItem;
                tmpSubEditor[index].serializedObject.ApplyModifiedProperties();
                tmpSubEditor[index].serializedObject.Update();
            }

            if (index >= subEditors.Length) tmpExpandProp.Add(expandProperty);
            else tmpExpandProp.Insert(index, expandProperty);

            subEditors = tmpSubEditor.ToArray();
            isExpanded = tmpIsExpanded.ToArray();
            handleRect = tmpHandleRect.ToArray();
            objectRect = tmpActionRect.ToArray();
            expandProp = tmpExpandProp.ToArray();
        }

        protected void MoveSubEditorsElement(int srcIndex, int dstIndex)
        {
            if (srcIndex == dstIndex) return;

            TEditor tmpSubEditor = subEditors[srcIndex];
            AnimBool tmpIsExpanded = isExpanded[srcIndex];
            Rect tmpHandleRect = handleRect[srcIndex];
            Rect tmpActionRect = objectRect[srcIndex];
            SerializedProperty tmpExpandProp = expandProp[srcIndex];

            if (dstIndex < srcIndex)
            {
                Array.Copy(subEditors, dstIndex, subEditors, dstIndex + 1, srcIndex - dstIndex);
                Array.Copy(isExpanded, dstIndex, isExpanded, dstIndex + 1, srcIndex - dstIndex);
                Array.Copy(handleRect, dstIndex, handleRect, dstIndex + 1, srcIndex - dstIndex);
                Array.Copy(objectRect, dstIndex, objectRect, dstIndex + 1, srcIndex - dstIndex);
                Array.Copy(expandProp, dstIndex, expandProp, dstIndex + 1, srcIndex - dstIndex);
            }
            else
            {
                Array.Copy(subEditors, srcIndex + 1, subEditors, srcIndex, dstIndex - srcIndex);
                Array.Copy(isExpanded, srcIndex + 1, isExpanded, srcIndex, dstIndex - srcIndex);
                Array.Copy(handleRect, srcIndex + 1, handleRect, srcIndex, dstIndex - srcIndex);
                Array.Copy(objectRect, srcIndex + 1, objectRect, srcIndex, dstIndex - srcIndex);
                Array.Copy(expandProp, srcIndex + 1, expandProp, srcIndex, dstIndex - srcIndex);
            }

            subEditors[dstIndex] = tmpSubEditor;
            isExpanded[dstIndex] = tmpIsExpanded;
            handleRect[dstIndex] = tmpHandleRect;
            objectRect[dstIndex] = tmpActionRect;
            expandProp[dstIndex] = tmpExpandProp;
        }

        protected void RemoveSubEditorsElement(int removeIndex)
        {
            List<TEditor> tmpSubEditor = new List<TEditor>(subEditors);
            List<AnimBool> tmpIsExpanded = new List<AnimBool>(isExpanded);
            List<Rect> tmpHandleRect = new List<Rect>(handleRect);
            List<Rect> tmpActionRect = new List<Rect>(objectRect);
            List<SerializedProperty> tmpExpandProp = new List<SerializedProperty>(expandProp);

            tmpSubEditor.RemoveAt(removeIndex);
            tmpIsExpanded.RemoveAt(removeIndex);
            tmpHandleRect.RemoveAt(removeIndex);
            tmpActionRect.RemoveAt(removeIndex);
            tmpExpandProp.RemoveAt(removeIndex);

            subEditors = tmpSubEditor.ToArray();
            isExpanded = tmpIsExpanded.ToArray();
            handleRect = tmpHandleRect.ToArray();
            objectRect = tmpActionRect.ToArray();
            expandProp = tmpExpandProp.ToArray();
        }

        protected virtual void Setup(TEditor editor, int editorIndex)
        {
        }
    }
}