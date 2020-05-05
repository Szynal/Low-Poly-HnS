using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterPlayerStayTimeout : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Stay Timeout";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public float duration = 2.0f;
        private float startTime;
        private bool hasBeenExecuted;

        private void OnTriggerEnter(Collider c)
        {
            if (IsColliderPlayer(c))
            {
                startTime = Time.time;
                hasBeenExecuted = false;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (IsColliderPlayer(c))
            {
                startTime = Time.time;
            }
        }

        private void OnTriggerStay(Collider c)
        {
            bool timeout = startTime + duration < Time.time;
            if (IsColliderPlayer(c) && timeout && !hasBeenExecuted)
            {
                hasBeenExecuted = true;
                ExecuteTrigger(c.gameObject);
            }
        }
    }
}