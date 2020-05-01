using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class ConditionListVariableCount : ICondition
    {
        public enum Comparison
        {
            Equals,
            Different,
            LessThan,
            GreaterThan
        }

        public HelperListVariable listVariable = new HelperListVariable();

        [Space] public Comparison comparison = Comparison.Equals;
        public int count = 0;

        public override bool Check(GameObject target)
        {
            ListVariables list = listVariable.GetListVariables(target);
            if (list == null) return false;
            int listCount = list.variables.Count;

            switch (comparison)
            {
                case Comparison.Equals: return listCount == count;
                case Comparison.Different: return listCount != count;
                case Comparison.LessThan: return listCount < count;
                case Comparison.GreaterThan: return listCount > count;
            }

            return false;
        }

#if UNITY_EDITOR
        public static new string NAME = "Variables/List Variables Count";

        private const string NODE_TITLE = "List Variable {0} {1} {2}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                listVariable,
                comparison,
                count
            );
        }

#endif
    }
}