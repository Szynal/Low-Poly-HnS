using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionChangeTag : IAction
    {
        public TargetGameObject target = new TargetGameObject();

        [Space] [TagSelector] public string newTag = "";

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.target.GetGameObject(target);
            if (go) go.tag = newTag;

            return true;
        }

#if UNITY_EDITOR

        public static new string NAME = "Object/Change Tag";
        private const string NODE_TITLE = "Change {0} tag to {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                target,
                tag
            );
        }

#endif
    }
}