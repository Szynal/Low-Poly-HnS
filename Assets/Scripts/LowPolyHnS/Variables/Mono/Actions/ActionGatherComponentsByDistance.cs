using System.Collections.Generic;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionGatherComponentsByDistance : IAction
    {
        [Tooltip("Components require a Collider to be gathered")]
        public string component = "Character";

        [Space] public TargetGameObject origin = new TargetGameObject(TargetGameObject.Target.Player);
        public NumberProperty radius = new NumberProperty(10f);

        [Space] public HelperListVariable listVariables = new HelperListVariable();

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform originTransform = origin.GetTransform(target);
            if (originTransform == null) return true;

            Collider[] colliders = GatherColliders(target);
            List<GameObject> array = new List<GameObject>();
            for (int i = 0; i < colliders.Length; ++i)
            {
                Component element = colliders[i].gameObject.GetComponent(component);
                if (element != null) array.Add(colliders[i].gameObject);
            }

            array.Sort((x, y) =>
            {
                float distanceX = Vector3.Distance(originTransform.position, x.transform.position);
                float distanceY = Vector3.Distance(originTransform.position, y.transform.position);
                return distanceX.CompareTo(distanceY);
            });

            ListVariables list = listVariables.GetListVariables(target);
            VariablesManager.ListClear(list);

            foreach (GameObject element in array)
            {
                VariablesManager.ListPush(list, ListVariables.Position.Last, element);
            }

            return true;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual int FilterLayerMask()
        {
            return -1;
        }

        protected Collider[] GatherColliders(GameObject target)
        {
            Transform transformOrigin = origin.GetTransform(target);
            if (transformOrigin == null) return new Collider[0];

            Vector3 position = transformOrigin.position;
            QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal;
            int layerMask = FilterLayerMask();

            return Physics.OverlapSphere(position, radius.GetValue(target), layerMask, query);
        }

#if UNITY_EDITOR
        public static new string NAME = "Variables/Gather Components by Distance";
        private const string NODE_TITLE = "Gather {0} by distance to {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                component,
                origin
            );
        }
#endif
    }
}