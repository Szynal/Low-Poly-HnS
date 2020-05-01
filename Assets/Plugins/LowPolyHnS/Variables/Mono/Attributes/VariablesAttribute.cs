using UnityEngine;

namespace LowPolyHnS.Variables
{
    public class VariableFilterAttribute : PropertyAttribute
    {
        public Variable.DataType[] types =
        {
            Variable.DataType.Number
        };

        // INITIALIZER: ---------------------------------------------------------------------------

        public VariableFilterAttribute(params Variable.DataType[] types)
        {
            this.types = types;
        }
    }
}