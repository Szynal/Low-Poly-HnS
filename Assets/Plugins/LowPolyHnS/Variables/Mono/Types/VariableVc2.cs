using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableVc2 : VariableGeneric<Vector2>
    {
        public new const string NAME = "Vector2";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableVc2()
        {
        }

        public VariableVc2(Vector2 value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Vector2);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Vector2;
        }
    }
}