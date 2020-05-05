using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterInvisible : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Become Invisible";
#endif

        private void OnBecameInvisible()
        {
            ExecuteTrigger(gameObject);
        }
    }
}