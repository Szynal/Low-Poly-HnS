using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
#endif

    [AddComponentMenu("")]
    public class ActionToggleActive : IAction
    {
        public TargetGameObject Target = new TargetGameObject();

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetValue = Target.GetGameObject(target);
            if (targetValue != null)
            {
                targetValue.SetActive(!targetValue.activeSelf);
            }

            return true;
        }
    }
}