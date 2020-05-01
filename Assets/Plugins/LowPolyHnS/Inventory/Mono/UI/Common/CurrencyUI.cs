using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Currency")]
    public class CurrencyUI : MonoBehaviour
    {
        public Text text;

        private bool isExitingApplication;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void OnEnable()
        {
            InventoryManager.Instance.eventChangePlayerCurrency.AddListener(OnUpdate);
            OnUpdate();
        }

        private void OnDisable()
        {
            if (isExitingApplication) return;
            InventoryManager.Instance.eventChangePlayerCurrency.RemoveListener(OnUpdate);
        }

        private void OnApplicationQuit()
        {
            isExitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnUpdate()
        {
            int currency = InventoryManager.Instance.GetCurrency();
            text.text = currency.ToString();
        }
    }
}