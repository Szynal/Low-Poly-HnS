namespace LowPolyHnS.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [ExecuteInEditMode]
    [AddComponentMenu("Game Creator/Actions", 0)]
    public class Actions : MonoBehaviour
    {
        public static bool IS_BLOCKING_ACTION_RUNNING = false;
        public class ExecuteEvent : UnityEvent<GameObject> { }

        public int currentID = 0;
        public int instanceID = 0;


        public IActionsList actionsList;
    }
}
