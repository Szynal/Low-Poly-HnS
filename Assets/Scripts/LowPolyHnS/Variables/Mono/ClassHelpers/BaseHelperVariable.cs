using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public abstract class BaseHelperVariable
    {
        private const int DEFAULT_MASK = ~0;

        // PROPERTIES: ----------------------------------------------------------------------------

        public int allowTypesMask = DEFAULT_MASK;

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract object Get(GameObject invoker = null);
        public abstract void Set(object value, GameObject invoker = null);

        public abstract string ToStringValue(GameObject invoker = null);
        public abstract Variable.DataType GetDataType(GameObject invoker = null);
    }
}