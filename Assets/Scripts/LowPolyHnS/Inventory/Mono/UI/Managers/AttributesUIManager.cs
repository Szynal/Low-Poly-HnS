using System.Globalization;
using LowPolyHnS.Characters;
using LowPolyHnS.Core.Hooks;
using TMPro;
using UnityEngine;

namespace LowPolyHnS.Attributes
{
    public class AttributesUIManager : MonoBehaviour
    {
        public TextMeshProUGUI Strength = null;
        public TextMeshProUGUI Agility = null;
        public TextMeshProUGUI Intelligence = null;

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
        }

        public void UpdateAttributes(PlayerCharacter playerCharacter)
        {
            Strength.text = playerCharacter.Strength.Value.ToString(CultureInfo.InvariantCulture);
            Agility.text = playerCharacter.Agility.Value.ToString(CultureInfo.InvariantCulture);
            Intelligence.text = playerCharacter.Intelligence.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}