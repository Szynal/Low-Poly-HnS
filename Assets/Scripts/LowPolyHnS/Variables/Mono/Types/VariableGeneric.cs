using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public abstract class VariableGeneric<T> : VariableBase
    {
        public new const string NAME = "Generic";

        [SerializeField] private T value;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected VariableGeneric(T value = default)
        {
            this.value = value;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get()
        {
            return value;
        }

        public void Set(T value)
        {
            this.value = value;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Null;
        }
    }
}