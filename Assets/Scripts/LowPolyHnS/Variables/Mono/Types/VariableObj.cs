using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableObj : VariableGeneric<GameObject>
    {
        public new const string NAME = "Game Object";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableObj()
        {
        }

        public VariableObj(GameObject value) : base(value)
        {
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override bool CanSave()
        {
            return CanSaveType(Variable.DataType.GameObject);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.GameObject;
        }
    }
}