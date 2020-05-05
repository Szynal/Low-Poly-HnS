using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class Vector3Property : BaseProperty<Vector3>
    {
        public Vector3Property()
        {
        }

        public Vector3Property(Vector3 value) : base(value)
        {
        }
    }
}