using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableCol : VariableGeneric<Color>
    {
        public new const string NAME = "Color";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableCol() : base(Color.white)
        {
        }

        public VariableCol(Color value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Color);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Color;
        }
    }
}