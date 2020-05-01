using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterMouseExit : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Exit";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        private void OnMouseExit()
        {
            ExecuteTrigger(gameObject);
        }
    }
}