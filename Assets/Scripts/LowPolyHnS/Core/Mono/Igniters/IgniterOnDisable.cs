using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnDisable : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Disable";
#endif

        private void OnDisable()
        {
            ExecuteTrigger(gameObject);
        }
    }
}