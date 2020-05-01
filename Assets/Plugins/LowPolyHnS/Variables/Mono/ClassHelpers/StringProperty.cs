using System;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class StringProperty : BaseProperty<string>
    {
        public StringProperty()
        {
        }

        public StringProperty(string value) : base(value)
        {
        }
    }
}