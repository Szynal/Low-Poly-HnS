using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class ActionsList : MonoBehaviour
    {
        public class ActionCoroutine
        {
            public Coroutine Coroutine { get; }
            public object Result { get; private set; }

            private IEnumerator target;

            public ActionCoroutine(IEnumerator target)
            {
                this.target = target;
                Coroutine = CoroutinesManager.Instance.StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                while (target.MoveNext())
                {
                    Result = target.Current;
                    yield return Result;
                }
            }

            public void Stop()
            {
                CoroutinesManager.Instance.StopCoroutine(Coroutine);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Action[] actions = new Action[0];
        public int executingIndex = -1;
        public bool isExecuting;

        private ActionCoroutine actionCoroutine;
        private bool cancelExecution;

        // CONSTRUCTORS: --------------------------------------------------------------------------

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spActions = null;
            for (int i = 0; i < actions.Length; ++i)
            {
                Action action = actions[i];
                if (action != null && action.gameObject != gameObject)
                {
                    Action newAction = gameObject.AddComponent(action.GetType()) as Action;
                    EditorUtility.CopySerialized(action, newAction);

                    if (spActions == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spActions = serializedObject.FindProperty("actions");
                    }

                    spActions.GetArrayElementAtIndex(i).objectReferenceValue = newAction;
                }
            }

            if (spActions != null) spActions.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Execute(GameObject target, System.Action callback, params object[] parameters)
        {
            isExecuting = true;
            CoroutinesManager.Instance.StartCoroutine(
                ExecuteCoroutine(target, callback, parameters)
            );
        }

        public IEnumerator ExecuteCoroutine(GameObject target, System.Action callback, params object[] parameters)
        {
            isExecuting = true;
            cancelExecution = false;

            for (int i = 0; i < actions.Length && !cancelExecution; ++i)
            {
                if (actions[i] == null) continue;

                executingIndex = i;

                if (!actions[i].InstantExecute(target, actions, i, parameters))
                {
                    actionCoroutine = new ActionCoroutine(
                        actions[i].Execute(target, actions, i, parameters)
                    );

                    yield return actionCoroutine.Coroutine;

                    if (actionCoroutine == null || actionCoroutine.Result == null)
                    {
                        yield break;
                    }

                    if (actionCoroutine.Result is int)
                    {
                        i += (int) actionCoroutine.Result;
                    }
                }

                if (i >= actions.Length) break;
                if (i < 0) i = -1;
            }

            executingIndex = -1;
            isExecuting = false;

            if (callback != null) callback();
        }

        public void Cancel()
        {
            if (!isExecuting) return;
            cancelExecution = true;
        }

        public void Stop()
        {
            Cancel();
            if (!isExecuting) return;

            actions[executingIndex].Stop();
            executingIndex = 0;
        }
    }
}