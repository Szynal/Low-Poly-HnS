using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterCollisionEnter : Igniter
    {
        public Collider otherCollider;

#if UNITY_EDITOR
        public new static string NAME = "Object/On Collision Enter";
        public new static string COMMENT = "Leave empty to collide with any object";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void OnCollisionEnter(Collision c)
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