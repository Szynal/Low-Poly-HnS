using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class BoolProperty : BaseProperty<bool>
    {
        public BoolProperty()
        {
        }

        public BoolProperty(bool value) : base(value)
        {
        }
    }
}