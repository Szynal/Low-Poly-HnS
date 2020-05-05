using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionListVariableClear : IAction
    {
        public enum ClearType
        {
            ClearAll,
            ClearEmpty
        }

        public ClearType clear = ClearType.ClearAll;
        public HelperListVariable listVariables = new HelperListVariable();

        // EXECUTE METHOD: ------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = listVariables.GetListVariables(target);
            if (list == null || list.variables.Count == 0) return true;

            for (int i = list.variables.Count - 1; i >= 0; --i)
            {
                switch (clear)
                {
                    case ClearType.ClearAll:
                        list.Remove(i);
                        break;

                    case ClearType.ClearEmpty:
                        Variable variable = list.Get(i);
                        if (variable == null || variable.Get().Equals(null))
                        {
                            list.Remove(i);
                        }

                        break;
                }
            }

            return true;
        }

#if UNITY_EDITOR

        private const string NODE_TITLE = "{0} from {1}";
        public static new string NAME = "Variables/Clear List Variables";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                ObjectNames.NicifyVariableName(clear.ToString()),
                listVariables
            );
        }

#endif
    }
}