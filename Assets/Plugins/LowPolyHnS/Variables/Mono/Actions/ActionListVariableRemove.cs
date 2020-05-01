using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class ActionListVariableRemove : IAction
    {
        public HelperGetListVariable listVariables = new HelperGetListVariable();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = listVariables.GetListVariables(target);
            if (list == null || list.variables.Count == 0) return true;

            list.Remove(listVariables.select, listVariables.index);
            return true;
        }

#if UNITY_EDITOR

        private const string NODE_TITLE = "Remove {0}";
        public static new string NAME = "Variables/Remove from List Variables";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                listVariables
            );
        }

#endif
    }
}