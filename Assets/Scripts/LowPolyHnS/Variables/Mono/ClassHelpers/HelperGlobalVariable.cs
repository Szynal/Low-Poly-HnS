using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class HelperGlobalVariable : BaseHelperVariable
    {
        public string name;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(GameObject invoker = null)
        {
            return VariablesManager.GetGlobal(name);
        }

        public override void Set(object value, GameObject invoker = null)
        {
            VariablesManager.SetGlobal(name, value);
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return name;
        }

        public override string ToStringValue(GameObject invoker = null)
        {
            object value = VariablesManager.GetGlobal(name);
            return value != null ? value.ToString() : "null";
        }

        public override Variable.DataType GetDataType(GameObject invoker = null)
        {
            return VariablesManager.GetGlobalType(name, true);
        }
    }
}