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

        #region PROPERTIES

        public Action[] Actions = new Action[0];
        public int ExecutingIndex = -1;
        public bool IsExecuting;

        private ActionCoroutine actionCoroutine;
        private bool cancelExecution;

        #endregion


#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spActions = null;
            for (int i = 0; i < Actions.Length; ++i)
            {
                Action action = Actions[i];
                if (action == null || action.gameObject == gameObject)
                {
                    continue;
                }

                Action newAction = gameObject.AddComponent(action.GetType()) as Action;
                EditorUtility.CopySerialized(action, newAction);

                if (spActions == null)
                {
                    SerializedObject serializedObject = new SerializedObject(this);
                    spActions = serializedObject.FindProperty("actions");
                }

                spActions.GetArrayElementAtIndex(i).objectReferenceValue = newAction;
            }

            spActions?.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        #region PUBLIC METHODS

        public void Execute(GameObject target, System.Action callback, params object[] parameters)
        {
            IsExecuting = true;
            CoroutinesManager.Instance.StartCoroutine(ExecuteCoroutine(target, callback, parameters));
        }

        public IEnumerator ExecuteCoroutine(GameObject target, System.Action callback, params object[] parameters)
        {
            IsExecuting = true;
            cancelExecution = false;

            for (int i = 0; i < Actions.Length && !cancelExecution; ++i)
            {
                if (Actions[i] == null) continue;

                ExecutingIndex = i;

                if (Actions[i].InstantExecute(target, Actions, i, parameters) == false)
                {
                    actionCoroutine = new ActionCoroutine(Actions[i].Execute(target, Actions, i, parameters));

                    yield return actionCoroutine.Coroutine;

                    switch (actionCoroutine?.Result)
                    {
                        case null:
                            yield break;
                        case int result:
                            i += result;
                            break;
                    }
                }

                if (i >= Actions.Length) break;
                if (i < 0) i = -1;
            }

            ExecutingIndex = -1;
            IsExecuting = false;

            callback?.Invoke();
        }

        public void Cancel()
        {
            if (IsExecuting == false)
            {
                return;
            }

            cancelExecution = true;
        }

        public void Stop()
        {
            Cancel();
            if (IsExecuting == false)
            {
                return;
            }

            Actions[ExecutingIndex].Stop();
            ExecutingIndex = 0;
        }

        #endregion
    }
}