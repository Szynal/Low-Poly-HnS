using System.Globalization;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Damage
{
    public class DamageSystem
    {
        public float Value { get; }

        public enum DamageType
        {
            Physical,
            Magic
        }

        private Item weapon;
        public float MinDamageValue;
        public float MaxDamageValue;
        private float AverageDamageValue => (MinDamageValue + MaxDamageValue) / 2;

        public DamageType TypeOfDamage = DamageType.Physical;

        public DamageSystem(float strength, Item item)
        {
            weapon = item;
            if (weapon == null)
            {
                Debug.Log("NULL");

                MinDamageValue = 1f + (strength - 1) / 10;
                MaxDamageValue = 2f + (strength - 1) / 10;
            }
            else
            {
                Debug.Log("OHO");

                MinDamageValue = weapon.MinDamageValue + (strength - 1) / 10;
                MaxDamageValue = weapon.MaxDamageValue + (strength - 1) / 10;
            }

            Value = PhysicalDamageFormula(strength);
        }

        public float PhysicalDamageFormula(float strength)
        {
            return Random.Range(MinDamageValue, MaxDamageValue);
        }

        public string GetMinDamageValueInfo()
        {
            return MinDamageValue.ToString(CultureInfo.InvariantCulture);
        }

        public string GetMaxDamageValueInfo()
        {
            return MaxDamageValue.ToString(CultureInfo.InvariantCulture);
        }

        public string GetAverageDamageValue()
        {
            return AverageDamageValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}