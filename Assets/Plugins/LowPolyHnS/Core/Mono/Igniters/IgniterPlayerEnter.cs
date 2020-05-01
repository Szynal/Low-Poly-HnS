using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterPlayerEnter : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Enter";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        private void OnTriggerEnter(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                ExecuteTrigger(c.gameObject);
            }
        }
    }
}