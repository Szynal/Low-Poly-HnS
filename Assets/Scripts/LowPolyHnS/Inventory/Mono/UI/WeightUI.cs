using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Weight")]
    public class WeightUI : MonoBehaviour
    {
        public Text text;

        [Tooltip("Use {0} for the current inventory weight and {1} for the total available")]
        public string format = "{0} / {1}";

        private bool isExitingApplication;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void OnEnable()
        {
            InventoryManager.Instance.eventChangePlayerInventory.AddListener(OnUpdate);
            OnUpdate();
        }

        private void OnDisable()
        {
            if (isExitingApplication) return;
            InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(OnUpdate);
        }

        private void OnApplicationQuit()
        {
            isExitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnUpdate()
        {
            float curWeight = InventoryManager.Instance.GetCurrentWeight();
            GameObject player = HookPlayer.Instance == null ? null : HookPlayer.Instance.gameObject;

            float maxWeight = DatabaseInventory
                .Load()
                .inventorySettings
                .maxInventoryWeight
                .GetValue(player);

            text.text = string.Format(format, curWeight, maxWeight);
        }
    }
}