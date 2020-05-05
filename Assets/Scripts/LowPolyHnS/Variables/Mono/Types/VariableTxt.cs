using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableTxt : VariableGeneric<Texture2D>
    {
        public new const string NAME = "Texture2D";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableTxt()
        {
        }

        public VariableTxt(Texture2D value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.Texture2D);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Texture2D;
        }
    }
}