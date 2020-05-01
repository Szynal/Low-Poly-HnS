using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class Texture2DProperty : BaseProperty<Texture2D>
    {
        public Texture2DProperty()
        {
        }

        public Texture2DProperty(Texture2D value) : base(value)
        {
        }
    }
}