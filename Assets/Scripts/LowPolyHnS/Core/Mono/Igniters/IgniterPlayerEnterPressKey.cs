using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterPlayerEnterPressKey : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Enter Key";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public KeyCode keyCode = KeyCode.E;
        private bool playerInside;

        private void Update()
        {
            if (playerInside && Input.GetKeyDown(keyCode))
            {
                if (HookPlayer.Instance == null) return;
                ExecuteTrigger(HookPlayer.Instance.gameObject);
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                playerInside = true;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                playerInside = false;
            }
        }
    }
}