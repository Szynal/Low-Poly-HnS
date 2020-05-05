using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnEnable : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Enable";
#endif

        private new void OnEnable()
        {
            base.OnEnable();
            ExecuteTrigger(gameObject);
        }
    }
}