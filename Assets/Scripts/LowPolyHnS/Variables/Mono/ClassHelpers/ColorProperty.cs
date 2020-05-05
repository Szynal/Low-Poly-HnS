using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class ColorProperty : BaseProperty<Color>
    {
        public ColorProperty()
        {
        }

        public ColorProperty(Color value) : base(value)
        {
        }
    }
}