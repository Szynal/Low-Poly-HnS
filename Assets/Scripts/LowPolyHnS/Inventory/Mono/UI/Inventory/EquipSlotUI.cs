using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Equip Slot")]
    public class EquipSlotUI : MonoBehaviour
    {
        private static DatabaseInventory DATABASE_INVENTORY;

        // PROPERTIES: ----------------------------------------------------------------------------

        [InventorySingleItemType] public int itemType = 1;

        public Image equipmentImage;
        public Text equipmentText;

        public Image frameImage;
        public Sprite onUnequipped;
        public Sprite onEquipped;
        public Sprite onHighlight;

        private bool isExittingApplication;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            SetupEvents(EventTriggerType.PointerClick, OnClick);
        }

        private void OnEnable()
        {
            InventoryManager.Instance.eventOnEquip.AddListener(OnEquip);
            InventoryManager.Instance.eventOnUnequip.AddListener(OnEquip);
            UpdateUI();
        }

        private void OnDisable()
        {
            if (isExittingApplication) return;
            InventoryManager.Instance.eventOnEquip.RemoveListener(OnEquip);
            InventoryManager.Instance.eventOnUnequip.RemoveListener(OnEquip);
        }

        private void OnApplicationQuit()
        {
            isExittingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnEquip(GameObject target, int itemID)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            Item item = null;
            if (HookPlayer.Instance != null)
            {
                item = InventoryManager.Instance.GetEquip(
                    HookPlayer.Instance.gameObject,
                    itemType
                );
            }

            if (item != null)
            {
                if (frameImage != null) frameImage.sprite = onEquipped;
                if (equipmentImage != null) equipmentImage.overrideSprite = item.sprite;
                if (equipmentText != null) equipmentText.text = item.itemName.GetText();
            }
            else
            {
                if (frameImage != null) frameImage.sprite = onUnequipped;
                if (equipmentImage != null) equipmentImage.overrideSprite = null;
                if (equipmentText != null) equipmentText.text = string.Empty;
            }
        }

        public void OnClick(BaseEventData eventData)
        {
            Item item = InventoryManager.Instance.GetEquip(
                HookPlayer.Instance.gameObject,
                itemType
            );

            if (item != null)
            {
                InventoryManager.Instance.Unequip(
                    HookPlayer.Instance.gameObject,
                    item.uuid
                );
            }
        }

        public void OnDrop(Item item)
        {
            if (item == null) return;
            InventoryManager.Instance.Equip(
                HookPlayer.Instance.gameObject,
                item.uuid,
                itemType
            );
        }

        // OTHER METHODS: -------------------------------------------------------------------------

        private void SetupEvents(EventTriggerType eventType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
            eventTriggerEntry.eventID = eventType;
            eventTriggerEntry.callback.AddListener(callback);

            EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();

            eventTrigger.triggers.Add(eventTriggerEntry);
        }
    }
}