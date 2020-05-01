using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public abstract class ICondition : MonoBehaviour
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

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        // PROPERTIES: ----------------------------------------------------------------------------

        public static string NAME = "General/Empty Condition";
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

        public void OnEnableEditor(Object condition)
        {
            serializedObject = new SerializedObject(condition);
            serializedObject.Update();

            OnEnableEditorChild();
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