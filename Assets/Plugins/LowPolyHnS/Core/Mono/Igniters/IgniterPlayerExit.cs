using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterPlayerExit : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Exit";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        private void OnTriggerExit(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                ExecuteTrigger(HookPlayer.Instance.gameObject);
            }
        }
    }
}