﻿using System.Globalization;
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
        private float MinDamageValue { get; }
        private float MaxDamageValue { get; }
        private float AverageDamageValue => (MinDamageValue + MaxDamageValue) / 2;

        public DamageType TypeOfDamage = DamageType.Physical;

        public DamageSystem(float strength, Item item)
        {
            if (weapon == null)
            {
                MinDamageValue = 1f;
                MaxDamageValue = 2f;
            }
            else
            {
                MinDamageValue = weapon.MinDamageValue;
                MaxDamageValue = weapon.MaxDamageValue;
            }

            Value = PhysicalDamageFormula(strength);
            weapon = item;
        }

        public float PhysicalDamageFormula(float strength)
        {
            return Random.Range(MinDamageValue, MaxDamageValue) * (strength / 10);
        }

        public string GetMinDamageValueInfo()
        {
            return MinDamageValue.ToString(CultureInfo.InvariantCulture);
        }

        public string GetMaxDamageValueInfo()
        {
            return MinDamageValue.ToString(CultureInfo.InvariantCulture);
        }

        public string GetAverageDamageValue()
        {
            return AverageDamageValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}