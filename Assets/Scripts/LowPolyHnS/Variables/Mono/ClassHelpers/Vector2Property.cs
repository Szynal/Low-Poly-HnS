using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class Vector2Property : BaseProperty<Vector2>
    {
        public Vector2Property()
        {
        }

        public Vector2Property(Vector2 value) : base(value)
        {
        }
    }
}