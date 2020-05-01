using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableBol : VariableGeneric<bool>
    {
        public new const string NAME = "Bool";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableBol()
        {
        }

        public VariableBol(bool value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Bool);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Bool;
        }
    }
}