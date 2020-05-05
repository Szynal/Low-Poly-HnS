using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterStart : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Start";
#endif

        private void Start()
        {
            ExecuteTrigger(gameObject);
        }
    }
}