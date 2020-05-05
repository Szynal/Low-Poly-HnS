using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public abstract class IActionNearest : IAction
    {
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeInVariable = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space] public TargetGameObject origin = new TargetGameObject(TargetGameObject.Target.Player);
        [Indent] public float radius = 10f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform originTransform = origin.GetTransform(target);
            if (originTransform == null) return true;

            Collider[] colliders = GatherColliders(target);

            GameObject nearestGameObject = null;
            float nearestDistance = Mathf.Infinity;

            for (int i = 0; i < colliders.Length; ++i)
            {
                GameObject item = colliders[i].gameObject;
                if (!FilterCondition(item)) continue;

                float distance = Vector3.Distance(item.transform.position, originTransform.position);
                if (distance < nearestDistance)
                {
                    nearestGameObject = item;
                    nearestDistance = distance;
                }
            }

            storeInVariable.Set(nearestGameObject, target);
            return true;
        }

        protected virtual bool FilterCondition(GameObject item)
        {
            return true;
        }

        protected virtual int FilterLayerMask()
        {
            return -1;
        }

        public Collider[] GatherColliders(GameObject target)
        {
            Transform transformOrigin = origin.GetTransform(target);
            if (transformOrigin == null) return new Collider[0];

            Vector3 position = transformOrigin.position;
            QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal;
            int layerMask = FilterLayerMask();

            return Physics.OverlapSphere(position, radius, layerMask, query);
        }
    }
}