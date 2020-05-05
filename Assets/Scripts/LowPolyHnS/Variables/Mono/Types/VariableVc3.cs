using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableVc3 : VariableGeneric<Vector3>
    {
        public new const string NAME = "Vector3";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableVc3()
        {
        }

        public VariableVc3(Vector3 value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Vector3);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Vector3;
        }
    }
}