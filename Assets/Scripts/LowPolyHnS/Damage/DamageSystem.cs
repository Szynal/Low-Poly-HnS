using LowPolyHnS.Attributes;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Damage
{
    public class DamageSystem : MonoBehaviour
    {
        public enum DamageType
        {
            Physical,
            Magic
        }

        public readonly Item Weapon;
        public readonly float MinDamageValue;
        public readonly float MaxDamageValue;
        public readonly float AverageDamageValue;
        public readonly float Value;

        public readonly AttributeModifierType Type;
        public readonly int Order;

        public DamageType TypeOfDamage = DamageType.Physical;

        public DamageSystem(AttributeModifier strength, float value, AttributeModifierType type, int order, Item item)
        {
            Value = PhysicalDamageFormula(strength);
            Type = type;
            Order = order;
            Weapon = item;
        }

        public float PhysicalDamageFormula(AttributeModifier strength)
        {
            return (Random.Range(MinDamageValue, MaxDamageValue) * (strength.Value / 10)) ;
        }
    }
}