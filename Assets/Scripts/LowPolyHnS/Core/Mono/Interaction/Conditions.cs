using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [ExecuteInEditMode]
    [AddComponentMenu("LowPolyHnS/Conditions", 0)]
    public class Conditions : MonoBehaviour
    {
        public Clause[] clauses = new Clause[0];
        public Actions defaultActions;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onInteract = new UnityEvent();

        // INTERACT METHOD: -----------------------------------------------------------------------

        public virtual void Interact()
        {
            Interact(null);
        }

        public virtual void Interact(GameObject target, params object[] parameters)
        {
            if (onInteract != null) onInteract.Invoke();
            for (int i = 0; i < clauses.Length; ++i)
            {
                if (clauses[i].CheckConditions(target, parameters))
                {
                    clauses[i].ExecuteActions(target, parameters);
                    return;
                }
            }

            if (defaultActions != null)
            {
                defaultActions.Execute(target);
            }
        }

        public virtual IEnumerator InteractCoroutine(GameObject target = null)
        {
            for (int i = 0; i < clauses.Length; ++i)
            {
                if (clauses[i].CheckConditions(target))
                {
                    Actions actions = clauses[i].actions;
                    if (actions != null)
                    {
                        Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                            actions.actionsList.ExecuteCoroutine(target, null)
                        );

                        yield return coroutine;
                    }

                    yield break;
                }
            }

            if (defaultActions != null)
            {
                Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                    defaultActions.actionsList.ExecuteCoroutine(target, null)
                );

                yield return coroutine;
            }
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            SerializedProperty spClauses = null;
            for (int i = 0; i < clauses.Length; ++i)
            {
                Clause clause = clauses[i];
                if (clause != null && clause.gameObject != gameObject)
                {
                    Clause newClause = gameObject.AddComponent(clause.GetType()) as Clause;
                    EditorUtility.CopySerialized(clause, newClause);

                    if (spClauses == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spClauses = serializedObject.FindProperty("clauses");
                    }

                    spClauses.GetArrayElementAtIndex(i).objectReferenceValue = newClause;
                }
            }

            if (spClauses != null) spClauses.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // GIZMO METHODS: -------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            int numClauses = clauses == null ? 0 : clauses.Length;
            switch (numClauses)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "Conditions/conditions0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "Conditions/conditions1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "Conditions/conditions2", true);
                    break;
                default:
                    Gizmos.DrawIcon(transform.position, "Conditions/conditions3", true);
                    break;
            }
        }
    }
}