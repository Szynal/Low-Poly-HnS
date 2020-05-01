using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [ExecuteInEditMode]
    [AddComponentMenu("LowPolyHnS/Actions", 0)]
    public class Actions : MonoBehaviour
    {
        public static bool IS_BLOCKING_ACTION_RUNNING;

        public class ExecuteEvent : UnityEvent<GameObject>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

#if UNITY_EDITOR
        public int currentID = 0;
        public int instanceID = 0;
#endif

        public IActionsList actionsList;

        [Tooltip("Only one foreground Actions collection can be executed at a given time.")]
        public bool runInBackground = true;

        [Tooltip("Useful for executing an Actions collection only once.")]
        public bool destroyAfterFinishing = false;

        private bool isDestroyed;

        // EVENTS: --------------------------------------------------------------------------------

        public ExecuteEvent onExecute = new ExecuteEvent();
        public UnityEvent onFinish = new UnityEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void OnDestroy()
        {
            isDestroyed = true;
            if (actionsList != null && actionsList.isExecuting && !runInBackground)
            {
                IS_BLOCKING_ACTION_RUNNING = false;
            }
        }

        private void Awake()
        {
            if (actionsList == null) actionsList = gameObject.AddComponent<IActionsList>();
        }

        private void OnEnable()
        {
            if (actionsList == null) actionsList = gameObject.AddComponent<IActionsList>();

#if UNITY_EDITOR
            if (actionsList.gameObject != gameObject)
            {
                IActionsList newActionsList = gameObject.AddComponent<IActionsList>();
                EditorUtility.CopySerialized(actionsList, newActionsList);

                SerializedObject serializedObject = new SerializedObject(this);
                serializedObject.FindProperty("actionsList").objectReferenceValue = newActionsList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (actionsList == null)
            {
                actionsList = gameObject.AddComponent<IActionsList>();
            }
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Execute(GameObject target, params object[] parameters)
        {
            if (actionsList.isExecuting) return;

            if (!runInBackground)
            {
                if (IS_BLOCKING_ACTION_RUNNING) return;
                IS_BLOCKING_ACTION_RUNNING = true;
            }

            if (onExecute != null) onExecute.Invoke(target);
            actionsList.Execute(target, OnFinish, parameters);
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
            if (onFinish != null) onFinish.Invoke();
            if (!runInBackground) IS_BLOCKING_ACTION_RUNNING = false;

            if (destroyAfterFinishing && !isDestroyed)
            {
                Destroy(gameObject);
            }
        }

        public virtual void Stop()
        {
            if (actionsList == null) return;
            actionsList.Stop();
        }

        // GIZMO METHODS: -------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            int numActions = actionsList == null || actionsList.actions == null
                ? 0
                : actionsList.actions.Length;

            switch (numActions)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "Actions/actions0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "Actions/actions1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "Actions/actions2", true);
                    break;
                default:
                    Gizmos.DrawIcon(transform.position, "Actions/actions3", true);
                    break;
            }
        }
    }
}