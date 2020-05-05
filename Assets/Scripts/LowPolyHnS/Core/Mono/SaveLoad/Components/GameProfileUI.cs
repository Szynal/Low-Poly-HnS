using System;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/UI/Game Profile", 100)]
    public class GameProfileUI : MonoBehaviour
    {
        public NumberProperty profile = new NumberProperty(1);

        public Text textProfile;
        public string formatProfile = "{0:00}";

        public Text textDate;
        public string formatDate = "dd/MM/yyyy HH:mm";

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            UpdateUI();
            SaveLoadManager.Instance.onSave += UpdateUI;
        }

        private void OnDestroy()
        {
            SaveLoadManager.Instance.onSave -= UpdateUI;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateUI(int updateProfile = -1)
        {
            int profileNumber = Mathf.RoundToInt(profile.GetValue(gameObject));

            if (updateProfile != -1 && profileNumber != updateProfile) return;
            SavesData.Profile data = SaveLoadManager.Instance.GetProfileInfo(profileNumber);

            if (textProfile != null)
            {
                textProfile.text = string.Format(
                    formatProfile,
                    profileNumber
                );
            }

            if (textDate != null && data != null)
            {
                DateTime date = DateTime.Parse(data.date);
                textDate.text = date.ToString(formatDate);
            }
        }
    }
}