using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class ActionListVariableAdd : IAction
    {
        public HelperGetListVariable listVariables = new HelperGetListVariable();
        [Space] public TargetGameObject item = new TargetGameObject();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = listVariables.GetListVariables(target);
            if (list == null) return true;

            GameObject elementGo = item.GetGameObject(target);
            if (elementGo == null) return true;

            list.Push(elementGo, listVariables.select, listVariables.index);
            return true;
        }

#if UNITY_EDITOR

        private const string NODE_TITLE = "Add {0} to {1}";
        public static new string NAME = "Variables/Add to List Variables";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                item,
                listVariables
            );
        }

#endif
    }
}