using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class EventDispatchManager : Singleton<EventDispatchManager>
    {
        [Serializable]
        public class Dispatcher : UnityEvent<GameObject>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, Dispatcher> events = new Dictionary<string, Dispatcher>();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            events = new Dictionary<string, Dispatcher>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Dispatch(string name, GameObject invoker)
        {
            RequireInit(ref name);
            events[name].Invoke(invoker);
        }

        public void Subscribe(string name, UnityAction<GameObject> callback)
        {
            RequireInit(ref name);
            events[name].AddListener(callback);
        }

        public void Unsubscribe(string name, UnityAction<GameObject> callback)
        {
            RequireInit(ref name);
            events[name].RemoveListener(callback);
        }

        public string[] GetSubscribedKeys()
        {
            int index = 0;
            string[] keys = new string[events.Keys.Count];

            foreach (string key in events.Keys)
            {
                keys[index] = key;
            }

            return keys;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RequireInit(ref string eventName)
        {
            eventName = eventName.Trim().Replace(" ", "-").ToLower();

            if (events.ContainsKey(eventName)) return;
            events.Add(eventName, new Dispatcher());
        }
    }
}