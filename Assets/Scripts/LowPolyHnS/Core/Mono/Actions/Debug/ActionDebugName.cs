using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionDebugName : IAction
    {
        public TargetGameObject _object = new TargetGameObject(TargetGameObject.Target.Invoker);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject instance = _object.GetGameObject(target);
            if (instance == null) return true;

            Debug.Log(instance.name);
            return true;
        }

#if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Name";
        private const string NODE_TITLE = "Debug.Log: Name of {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, _object);
        }

#endif
    }
}