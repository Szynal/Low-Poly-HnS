using System;
using System.Collections.Generic;
using System.Linq;

namespace LowPolyHnS.Attributes
{
    public class CharacterAttributes
    {
        public float BaseValue;

        private readonly List<AttributeModifier> statModifiers;

        public CharacterAttributes(float baseValue)
        {
            BaseValue = baseValue;
            statModifiers = new List<AttributeModifier>();
        }

        public float Value => CalculateFinalValue();

        public void AddModifier(AttributeModifier attributeModifier)
        {
            statModifiers.Add(attributeModifier);
        }

        public bool RemoveModifier(AttributeModifier attributeModifier)
        {
            return statModifiers.Remove(attributeModifier);
        }

        private float CalculateFinalValue()
        {
            float finalValue = BaseValue + statModifiers.Sum(t => t.Value);

            return (float) Math.Round(finalValue, 4);
        }
    }
}