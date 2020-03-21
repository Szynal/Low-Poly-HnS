using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [ExecuteInEditMode]
    [AddComponentMenu("Game Creator/Actions", 0)]
    public class Actions : MonoBehaviour
    {
        public static bool IS_BLOCKING_ACTION_RUNNING;

        public class ExecuteEvent : UnityEvent<GameObject>
        {
        }

#if UNITY_EDITOR
        public int CurrentID = 0;
        public int InstanceID = 0;
#endif

        public ActionsList ActionsList;

        [Tooltip("Only one foreground Actions collection can be executed at a given time.")]
        public bool RunInBackground = true;

        [Tooltip("Useful for executing an Actions collection only once.")]
        public bool DestroyAfterFinishing = false;

        private bool isDestroyed;


        #region EVENTS

        public ExecuteEvent OnExecute = new ExecuteEvent();
        public UnityEvent onFinish = new UnityEvent();

        #endregion


        public void OnDestroy()
        {
            isDestroyed = true;
            if (ActionsList != null && ActionsList.IsExecuting && !RunInBackground)
            {
                IS_BLOCKING_ACTION_RUNNING = false;
            }
        }

        private void Awake()
        {
            if (ActionsList == null) ActionsList = gameObject.AddComponent<ActionsList>();
        }

        private void OnEnable()
        {
            if (ActionsList == null) ActionsList = gameObject.AddComponent<ActionsList>();

#if UNITY_EDITOR

            if (ActionsList.gameObject == gameObject)
            {
                return;
            }

            ActionsList newActionsList = gameObject.AddComponent<ActionsList>();
            EditorUtility.CopySerialized(ActionsList, newActionsList);

            SerializedObject serializedObject = new SerializedObject(this);
            serializedObject.FindProperty("actionsList").objectReferenceValue = newActionsList;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ActionsList == null)
            {
                ActionsList = gameObject.AddComponent<ActionsList>();
            }
        }
#endif

        public virtual void Execute(GameObject target, params object[] parameters)
        {
            if (ActionsList.IsExecuting)
            {
                return;
            }

            if (RunInBackground == false)
            {
                if (IS_BLOCKING_ACTION_RUNNING)
                {
                    return;
                }

                IS_BLOCKING_ACTION_RUNNING = true;
            }

            OnExecute?.Invoke(target);
            ActionsList.Execute(target, OnFinish, parameters);
        }

        public virtual void Execute(params object[] parameters)
        {
            Execute(null, parameters);
        }

        public virtual void Execute()
        {
            Execute(null, new object[0]);
        }

        public virtual void ExecuteWithTarget(GameObject target)
        {
            Execute(target);
        }

        public virtual void OnFinish()
        {
            onFinish?.Invoke();

            if (RunInBackground == false)
            {
                IS_BLOCKING_ACTION_RUNNING = false;
            }

            if (DestroyAfterFinishing && !isDestroyed)
            {
                Destroy(gameObject);
            }
        }

        public virtual void Stop()
        {
            if (ActionsList == null) return;
            ActionsList.Stop();
        }

        private void OnDrawGizmos()
        {
            int numActions = ActionsList == null || ActionsList.Actions == null
                ? 0
                : ActionsList.Actions.Length;

            switch (numActions)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions2", true);
                    break;
                default:
                    Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions3", true);
                    break;
            }
        }
    }
}