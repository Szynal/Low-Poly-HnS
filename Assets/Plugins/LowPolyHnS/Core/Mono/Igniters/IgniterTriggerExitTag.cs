using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class IgniterTriggerExitTag : Igniter
    {
        [TagSelector] public string objectWithTag = "";

#if UNITY_EDITOR
        public new static string NAME = "Object/On Tag Exit";
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
            if (string.IsNullOrEmpty(objectWithTag) || c.CompareTag(objectWithTag))
            {
                storeSelf.Set(gameObject);
                storeCollider.Set(c.gameObject);

                ExecuteTrigger(c.gameObject);
            }
        }
    }
}