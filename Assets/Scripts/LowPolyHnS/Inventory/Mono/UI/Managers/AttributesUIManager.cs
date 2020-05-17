using System.Globalization;
using LowPolyHnS.Characters;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Damage;
using TMPro;
using UnityEngine;

namespace LowPolyHnS.Attributes
{
    public class AttributesUIManager : MonoBehaviour
    {
        public TextMeshProUGUI Strength = null;
        public TextMeshProUGUI Agility = null;
        public TextMeshProUGUI Intelligence = null;

        public TextMeshProUGUI FireResistance = null;
        public TextMeshProUGUI ColdResistance = null;
        public TextMeshProUGUI PoisonResistance = null;

        public TextMeshProUGUI DamageValue = null;

        public void UpdateAttributes()
        {
            if (HookPlayer.Instance == null)
            {
                return;
            }

            PlayerCharacter playerCharacter = HookPlayer.Instance.GetComponent<PlayerCharacter>();
            if (playerCharacter == null)
            {
                return;
            }

            Strength.text = playerCharacter.Strength.Value.ToString(CultureInfo.InvariantCulture);
            Agility.text = playerCharacter.Agility.Value.ToString(CultureInfo.InvariantCulture);
            Intelligence.text = playerCharacter.Intelligence.Value.ToString(CultureInfo.InvariantCulture);

            FireResistance.text = playerCharacter.FireResistance.Value.ToString(CultureInfo.InvariantCulture);
            ColdResistance.text = playerCharacter.ColdResistance.Value.ToString(CultureInfo.InvariantCulture);
            PoisonResistance.text = playerCharacter.PoisonResistance.Value.ToString(CultureInfo.InvariantCulture);

            DamageSystem damageSystem = playerCharacter.DamageSystem;

            if (damageSystem != null)
            {
                DamageValue.text = $"{damageSystem.GetMinDamageValueInfo()} - {damageSystem.GetMaxDamageValueInfo()}";
            }
        }

        public void UpdateAttributes(PlayerCharacter playerCharacter)
        {
            Strength.text = playerCharacter.Strength.Value.ToString(CultureInfo.InvariantCulture);
            Agility.text = playerCharacter.Agility.Value.ToString(CultureInfo.InvariantCulture);
            Intelligence.text = playerCharacter.Intelligence.Value.ToString(CultureInfo.InvariantCulture);

            FireResistance.text = $"{playerCharacter.FireResistance.Value.ToString(CultureInfo.InvariantCulture)} %";
            ColdResistance.text = $"{playerCharacter.ColdResistance.Value.ToString(CultureInfo.InvariantCulture)} %";
            PoisonResistance.text = $"{playerCharacter.PoisonResistance.Value.ToString(CultureInfo.InvariantCulture)} %";
        }
    }
}