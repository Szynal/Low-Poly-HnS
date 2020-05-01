using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterCursorRaycast : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Input/On Cursor Raycast";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public KeyCode key = KeyCode.Mouse0;

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        private void Update()
        {
            if (Input.GetKeyUp(key))
            {
                Ray ray = HookCamera.Instance.Get<Camera>().ScreenPointToRay(Input.mousePosition);

                int hitCount = Physics.RaycastNonAlloc(ray, hitBuffer);
                if (hitCount == 0) return;

                GameObject hitGO = hitBuffer[0].collider.gameObject;
                if (hitGO.GetInstanceID() == gameObject.GetInstanceID())
                {
                    storeCollider.Set(hitGO, gameObject);
                    ExecuteTrigger(gameObject);
                }
            }
        }
    }
}