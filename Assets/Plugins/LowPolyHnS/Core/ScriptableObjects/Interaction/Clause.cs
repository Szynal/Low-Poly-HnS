using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class Clause : MonoBehaviour
    {
        public string description = "Clause";
        public IConditionsList conditionsList;
        public Actions actions;

        public bool isExpanded = false;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onCheckConditions = new UnityEvent();
        public UnityEvent onExecuteActions = new UnityEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            if (conditionsList != null &&
                conditionsList.gameObject != gameObject)
            {
                IConditionsList newConditionsList = gameObject.AddComponent<IConditionsList>();
                EditorUtility.CopySerialized(conditionsList, newConditionsList);

                SerializedObject serializedObject = new SerializedObject(this);
                serializedObject.FindProperty("conditionsList").objectReferenceValue = newConditionsList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void OnValidate()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual bool CheckConditions(GameObject target = null, params object[] parameters)
        {
            if (onCheckConditions != null) onCheckConditions.Invoke();
            return conditionsList.Check(target, parameters);
        }

        public virtual void ExecuteActions(GameObject target = null, params object[] parameters)
        {
            if (actions != null)
            {
                if (onExecuteActions != null) onExecuteActions.Invoke();
                actions.Execute(target, parameters);
            }
        }
    }
}