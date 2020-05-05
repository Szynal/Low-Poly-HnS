using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterTriggerStayTimeout : Igniter
    {
        public Collider otherCollider;

#if UNITY_EDITOR
        public new static string NAME = "Object/On Trigger Stay Timeout";
        public new static bool REQUIRES_COLLIDER = true;
#endif

        public float duration = 2.0f;
        private float startTime;
        private bool hasBeenExecuted;

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space] [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void OnTriggerEnter(Collider c)
        {
            if (otherCollider == null)
            {
                startTime = Time.time;
                hasBeenExecuted = false;
            }
            else if (otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                startTime = Time.time;
                hasBeenExecuted = false;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (otherCollider == null)
            {
                startTime = Time.time;
            }
            else if (otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                startTime = Time.time;
            }
        }

        private void OnTriggerStay(Collider c)
        {
            bool timeout = startTime + duration < Time.time;
            bool collided = otherCollider == null ||
                            otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID();

            if (collided && timeout && !hasBeenExecuted)
            {
                storeSelf.Set(gameObject);
                storeCollider.Set(c.gameObject);

                hasBeenExecuted = true;
                ExecuteTrigger(c.gameObject);
            }
        }
    }
}