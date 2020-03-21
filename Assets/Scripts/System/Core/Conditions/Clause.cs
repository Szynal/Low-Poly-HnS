using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class Clause : MonoBehaviour
    {
        public string Description = "Clause";
        public ConditionsList ConditionsList;
        public Actions Actions;
        public bool IsExpanded = false;

        #region EVENTS

        public UnityEvent OnCheckConditions = new UnityEvent();
        public UnityEvent OnExecuteActions = new UnityEvent();

        #endregion


#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            if (ConditionsList == null || ConditionsList.gameObject == gameObject)
            {
                return;
            }

            ConditionsList newConditionsList = gameObject.AddComponent<ConditionsList>();
            EditorUtility.CopySerialized(ConditionsList, newConditionsList);

            SerializedObject serializedObject = new SerializedObject(this);
            serializedObject.FindProperty("conditionsList").objectReferenceValue = newConditionsList;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnValidate()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }
#endif

        public virtual bool CheckConditions(GameObject target = null, params object[] parameters)
        {
            OnCheckConditions?.Invoke();
            return ConditionsList.Check(target, parameters);
        }

        public virtual void ExecuteActions(GameObject target = null, params object[] parameters)
        {
            if (Actions == null)
            {
                return;
            }

            OnExecuteActions?.Invoke();

            Actions.Execute(target, parameters);
        }
    }
}