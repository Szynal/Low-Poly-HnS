using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableNum : VariableGeneric<float>
    {
        public new const string NAME = "Number";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableNum()
        {
        }

        public VariableNum(float value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Number);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Number;
        }
    }
}