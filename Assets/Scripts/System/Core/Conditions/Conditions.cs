using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [ExecuteInEditMode]
    [AddComponentMenu("LowPolyHnS/Conditions", 0)]
    public class Conditions : MonoBehaviour
    {
        public Clause[] Clauses = new Clause[0];
        public Actions DefaultActions;

        #region EVENTS

        public UnityEvent OnInteract = new UnityEvent();

        #endregion

        #region INTERACT METHOD

        public virtual void Interact()
        {
            Interact(null);
        }

        public virtual void Interact(GameObject target, params object[] parameters)
        {
            if (OnInteract != null) OnInteract.Invoke();
            for (int i = 0; i < Clauses.Length; ++i)
            {
                if (Clauses[i].CheckConditions(target, parameters))
                {
                    Clauses[i].ExecuteActions(target, parameters);
                    return;
                }
            }

            if (DefaultActions != null)
            {
                DefaultActions.Execute(target);
            }
        }

        public virtual IEnumerator InteractCoroutine(GameObject target = null)
        {
            foreach (Clause clause in Clauses)
            {
                if (clause.CheckConditions(target) == false)
                {
                    continue;
                }

                Actions actions = clause.Actions;

                if (actions == null)
                {
                    yield break;
                }

                Coroutine coroutine =
                    CoroutinesManager.Instance.StartCoroutine(actions.ActionsList.ExecuteCoroutine(target, null));

                yield return coroutine;

                yield break;
            }

            if (DefaultActions != null)
            {
                Coroutine coroutine =
                    CoroutinesManager.Instance.StartCoroutine(
                        DefaultActions.ActionsList.ExecuteCoroutine(target, null));

                yield return coroutine;
            }
        }

        #endregion


#if UNITY_EDITOR
        private void OnEnable()
        {
            SerializedProperty spClauses = null;
            for (int i = 0; i < Clauses.Length; ++i)
            {
                Clause clause = Clauses[i];
                if (clause == null || clause.gameObject == gameObject)
                {
                    continue;
                }

                Clause newClause = gameObject.AddComponent(clause.GetType()) as Clause;
                EditorUtility.CopySerialized(clause, newClause);

                if (spClauses == null)
                {
                    SerializedObject serializedObject = new SerializedObject(this);
                    spClauses = serializedObject.FindProperty("clauses");
                }

                spClauses.GetArrayElementAtIndex(i).objectReferenceValue = newClause;
            }

            spClauses?.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif


        #region GIZMO

        private void OnDrawGizmos()
        {
            int numClauses = Clauses == null ? 0 : Clauses.Length;
            switch (numClauses)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions2", true);
                    break;
                default:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions3", true);
                    break;
            }
        }

        #endregion
    }
}