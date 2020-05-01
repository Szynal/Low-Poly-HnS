using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class NumberProperty : BaseProperty<float>
    {
        public NumberProperty()
        {
        }

        public NumberProperty(float value) : base(value)
        {
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int GetInt(GameObject invoker)
        {
            return Mathf.FloorToInt(GetValue(invoker));
        }
    }
}