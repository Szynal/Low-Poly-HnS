using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    public abstract class Condition : MonoBehaviour
    {
        public virtual bool Check()
        {
            return true;
        }

        public virtual bool Check(GameObject target)
        {
            return Check();
        }

        public virtual bool Check(GameObject target, params object[] parameters)
        {
            return Check(target);
        }

#if UNITY_EDITOR


        #region PROPERTIES

        public static string Name = "General/Empty Condition";
        protected SerializedObject SerializedObject;
        public bool IsExpanded = false;

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

        public void OnEnableEditor(Object condition)
        {
            SerializedObject = new SerializedObject(condition);
            SerializedObject.Update();

            OnEnableEditorChild();
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