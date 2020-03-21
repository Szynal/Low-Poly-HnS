using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [ExecuteInEditMode]
    public abstract class Action : MonoBehaviour
    {
        public virtual bool InstantExecute(GameObject target, Action[] actions, int index)
        {
            return false;
        }

        public virtual bool InstantExecute(GameObject target, Action[] actions, int index, params object[] parameters)
        {
            return InstantExecute(target, actions, index);
        }

        public virtual IEnumerator Execute(GameObject target, Action[] actions, int index)
        {
            yield return 0;
        }

        public virtual IEnumerator Execute(GameObject target, Action[] actions, int index, params object[] parameters)
        {
            IEnumerator execute = Execute(target, actions, index);

            while (execute.MoveNext())
            {
                var result = execute.Current;
                yield return result;
            }
        }

        public virtual void Stop()
        {
        }

#if UNITY_EDITOR

        #region PROPERTIES

        public static string Name = "General/Empty Action";
        public bool IsExpanded = false;
        protected SerializedObject SerializedObject;

        #endregion


        #region METHODS

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
            SerializedObject = new SerializedObject(action);
            SerializedObject.Update();

            OnEnableEditorChild();
        }

        public void OnDisableEditor()
        {
            SerializedObject = null;
            OnDisableEditorChild();
        }

        public void OnInspectorGUIEditor()
        {
            if (SerializedObject == null) return;
            OnInspectorGUI();
        }

        #endregion

        #region VIRTUAL AND ABSTRACT METHODS

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
            if (SerializedObject.targetObject == null)
            {
                return;
            }

            SerializedObject.Update();
            SerializedProperty iterator = SerializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                switch (iterator.propertyPath)
                {
                    case "m_Script":
                    case "isExpanded":
                        continue;
                    default:
                        EditorGUILayout.PropertyField(iterator, true);
                        break;
                }
            }

            SerializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnableEditorChild()
        {
        }

        protected virtual void OnDisableEditorChild()
        {
        }

        #endregion


#endif
    }
}