using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class SpriteProperty : BaseProperty<Sprite>
    {
        public SpriteProperty()
        {
        }

        public SpriteProperty(Sprite value) : base(value)
        {
        }
    }
}