using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterMouseEnter : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Enter";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        private void OnMouseEnter()
        {
            ExecuteTrigger(gameObject);
        }
    }
}