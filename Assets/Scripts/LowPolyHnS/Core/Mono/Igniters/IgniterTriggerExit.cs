using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterTriggerExit : Igniter
    {
        public Collider otherCollider;

#if UNITY_EDITOR
        public new static string NAME = "Object/On Trigger Exit";
        public new static string COMMENT = "Leave empty to trigger any object";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void Start()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (otherCollider == null)
            {
                storeSelf.Set(gameObject);
                storeCollider.Set(c.gameObject);

                ExecuteTrigger(c.gameObject);
            }
            else if (otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                storeSelf.Set(gameObject);
                storeCollider.Set(c.gameObject);

                ExecuteTrigger(c.gameObject);
            }
        }
    }
}