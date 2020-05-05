using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class HelperGetListVariable : HelperListVariable
    {
        public ListVariables.Position select = ListVariables.Position.First;
        public int index = 0;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(GameObject invoker)
        {
            ListVariables list = GetListVariables(invoker);
            Variable result = VariablesManager.GetListItem(list, select, index);
            return result != null ? result.Get() : null;
        }

        public void Set(object value, GameObject invoker = null)
        {
            ListVariables list = GetListVariables(invoker);
            list.Push(value, select, index);
        }

        public GameObject GetGameObject(GameObject invoker)
        {
            ListVariables list = GetListVariables(invoker);
            return list != null ? list.gameObject : null;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return string.Format("list[{0}]", select.ToString());
        }

        public string ToStringValue(GameObject invoker)
        {
            object value = Get(invoker);
            return value != null ? value.ToString() : "null";
        }
    }
}