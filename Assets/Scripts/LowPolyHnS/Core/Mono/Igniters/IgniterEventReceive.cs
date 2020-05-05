using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterEventReceive : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Event Receive";
#endif

        [EventName] public string eventName = "my-event";

        [Space] public VariableProperty storeInvoker = new VariableProperty();

        private void Start()
        {
            EventDispatchManager.Instance.Subscribe(eventName, OnReceiveEvent);
        }

        private void OnDestroy()
        {
            if (EventDispatchManager.IS_EXITING) return;
            EventDispatchManager.Instance.Unsubscribe(eventName, OnReceiveEvent);
        }

        private void OnReceiveEvent(GameObject invoker)
        {
            storeInvoker.Set(invoker, invoker);
            ExecuteTrigger(invoker);
        }
    }
}