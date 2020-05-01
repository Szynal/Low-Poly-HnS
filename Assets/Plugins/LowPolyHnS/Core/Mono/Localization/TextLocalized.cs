using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Localization
{
    [AddComponentMenu("LowPolyHnS/UI/Text (Localized)", 20)]
    public class TextLocalized : Text
    {
        [LocStringNoTextAttribute] public LocString locString = new LocString("My Text...");

        private bool exitingApplication;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                UpdateText();
                LocalizationManager.Instance.onChangeLanguage += UpdateText;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                LocalizationManager.Instance.onChangeLanguage += UpdateText;
                UpdateText();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying && !exitingApplication)
            {
                if (LocalizationManager.Instance == null) return;
                LocalizationManager.Instance.onChangeLanguage -= UpdateText;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Application.isPlaying && !exitingApplication)
            {
                if (LocalizationManager.Instance == null) return;
                LocalizationManager.Instance.onChangeLanguage -= UpdateText;
            }
        }

        private void OnApplicationQuit()
        {
            exitingApplication = true;
        }

        // MAIN METHODS: --------------------------------------------------------------------------

        public void ChangeKey(string textKey)
        {
            locString.content = textKey;
            UpdateText();
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(locString.content)) return;
            text = locString.GetText();
        }
    }
}