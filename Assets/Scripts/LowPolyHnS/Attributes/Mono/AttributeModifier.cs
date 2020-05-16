using LowPolyHnS.Inventory;

namespace LowPolyHnS.Attributes
{
    public enum AttributeModifierType
    {
        Normal = 100,
        Percent = 200,
        PercentMultiply = 300
    }

    public class AttributeModifier
    {
        public readonly float Value;
        public readonly AttributeModifierType Type;
        public readonly int Order;
        public readonly Item Item;

        public AttributeModifier(float value, AttributeModifierType type, int order, Item item)
        {
            Value = value;
            Type = type;
            Order = order;
            Item = item;
        }

        public AttributeModifier(float value, AttributeModifierType type, Item item)
        {
            Value = value;
            Type = type;
            Item = item;
        }
    }
}