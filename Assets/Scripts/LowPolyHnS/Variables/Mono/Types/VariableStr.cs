using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableStr : VariableGeneric<string>
    {
        public new const string NAME = "String";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableStr()
        {
        }

        public VariableStr(string value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.String);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.String;
        }
    }
}