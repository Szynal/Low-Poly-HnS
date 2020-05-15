using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LowPolyHnS.Attributes
{
    [Serializable]
    public class CharacterAttributes
    {
        public float BaseValue;

        protected bool NeedRecalculate = true;
        protected float LastBaseValue;
        protected float RecentCalculation;

        public virtual float Value
        {
            get
            {
                if (NeedRecalculate || LastBaseValue != BaseValue)
                {
                    LastBaseValue = BaseValue;
                    RecentCalculation = CalculateFinalValue();
                    NeedRecalculate = false;
                }

                return RecentCalculation;
            }
        }

        protected readonly List<AttributeModifier> StatModifiersList;
        public readonly ReadOnlyCollection<AttributeModifier> StatModifiers;

        public CharacterAttributes()
        {
            StatModifiersList = new List<AttributeModifier>();
            StatModifiers = StatModifiersList.AsReadOnly();
        }

        public CharacterAttributes(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(AttributeModifier mod)
        {
            NeedRecalculate = true;
            StatModifiersList.Add(mod);
        }

        public virtual bool RemoveModifier(AttributeModifier mod)
        {
            if (StatModifiersList.Remove(mod))
            {
                NeedRecalculate = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var numRemovals = StatModifiersList.RemoveAll(mod => mod.Source == source);

            if (numRemovals <= 0) return false;

            NeedRecalculate = true;
            return true;
        }

        protected virtual int CompareModifierOrder(AttributeModifier attributeModifierA, AttributeModifier attributeModifierB)
        {
            if (attributeModifierA.Order < attributeModifierB.Order)
            {
                return -1;
            }

            return attributeModifierA.Order > attributeModifierB.Order ? 1 : 0;
        }

        protected virtual float CalculateFinalValue()
        {
            var finalValue = BaseValue;
            float sumPercentAdd = 0;

            StatModifiersList.Sort(CompareModifierOrder);

            for (var i = 0; i < StatModifiersList.Count; i++)
            {
                var mod = StatModifiersList[i];

                switch (mod.Type)
                {
                    case AttributeModifierType.Flat:
                    {
                        finalValue += mod.Value;
                        break;
                    }

                    case AttributeModifierType.PercentAdd:
                    {
                        sumPercentAdd += mod.Value;

                        if (i + 1 >= StatModifiersList.Count || StatModifiersList[i + 1].Type != AttributeModifierType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }

                        break;
                    }

                    case AttributeModifierType.PercentMultiply:
                    {
                        finalValue *= 1 + mod.Value;
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return (float) Math.Round(finalValue, 4);
        }
    }
}