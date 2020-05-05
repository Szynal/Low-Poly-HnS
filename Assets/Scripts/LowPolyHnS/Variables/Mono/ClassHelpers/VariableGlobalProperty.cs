using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableGlobalProperty
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public HelperGlobalVariable globalVariable = new HelperGlobalVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableGlobalProperty()
        {
            globalVariable = globalVariable ?? new HelperGlobalVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get()
        {
            return globalVariable.Get();
        }

        public void Set(object value)
        {
            globalVariable.Set(value);
        }

        public string GetVariableID()
        {
            return globalVariable.name;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return globalVariable.ToString();
        }

        public string ToStringValue()
        {
            return globalVariable.ToStringValue();
        }
    }
}