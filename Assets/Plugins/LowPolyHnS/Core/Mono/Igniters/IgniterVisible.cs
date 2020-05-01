using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterVisible : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Become Visible";
#endif

        private void OnBecameVisible()
        {
            ExecuteTrigger(gameObject);
        }
    }
}