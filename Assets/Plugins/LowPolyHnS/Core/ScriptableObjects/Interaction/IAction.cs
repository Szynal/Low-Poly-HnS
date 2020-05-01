using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [ExecuteInEditMode]
    public abstract class IAction : MonoBehaviour
    {
        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            return false;
        }

        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            return InstantExecute(target, actions, index);
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            yield return 0;
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            IEnumerator execute = Execute(target, actions, index);
            object result = null;

            while (execute.MoveNext())
            {
                result = execute.Current;
                yield return result;
            }
        }

        public virtual void Stop()
        {
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        // PROPERTIES: ----------------------------------------------------------------------------

        public static string NAME = "General/Empty Action";

        protected SerializedObject serializedObject;
        public bool isExpanded = false;

        // METHODS: -------------------------------------------------------------------------------

        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnValidate()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        public void OnEnableEditor(Object action)
        {
            serializedObject = new SerializedObject(action);
            serializedObject.Update();

            OnEnableEditorChild();
        }

        public void OnDisableEditor()
        {
            serializedObject = null;
            OnDisableEditorChild();
        }

        public void OnInspectorGUIEditor()
        {
            if (serializedObject == null) return;
            OnInspectorGUI();
        }

        // VIRTUAL AND ABSTRACT METHODS: ----------------------------------------------------------

        public virtual string GetNodeTitle()
        {
            return GetType().Name;
        }

        public virtual float GetOpacity()
        {
            return 1.0f;
        }

        public virtual void OnInspectorGUI()
        {
            if (serializedObject.targetObject != null)
            {
                serializedObject.Update();
                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if ("m_Script" == iterator.propertyPath) continue;
                    if ("isExpanded" == iterator.propertyPath) continue;
                    EditorGUILayout.PropertyField(iterator, true);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void OnEnableEditorChild()
        {
        }

        protected virtual void OnDisableEditorChild()
        {
        }

#endif
    }
}