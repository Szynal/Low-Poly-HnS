using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Hotbar Item")]
    public class HotbarUI : MonoBehaviour
    {
        [Space] public KeyCode keyCode = KeyCode.Space;

        [InventoryMultiItemType] public int acceptItemTypes = -1;

        [Space] public Image itemImage;
        public Text itemText;
        public Text itemDescription;

        [Space] public UnityEvent eventOnHoverEnter;
        public UnityEvent eventOnHoverExit;

        private Item item;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            SetupEvents(EventTriggerType.PointerEnter, OnPointerEnter);
            SetupEvents(EventTriggerType.PointerExit, OnPointerExit);

            UpdateUI();
        }

        protected void SetupEvents(EventTriggerType eventType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
            eventTriggerEntry.eventID = eventType;
            eventTriggerEntry.callback.AddListener(callback);

            EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();

            eventTrigger.triggers.Add(eventTriggerEntry);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        private void Update()
        {
            if (item == null) return;
            if (!Input.GetKeyDown(keyCode)) return;

            InventoryManager.Instance.ConsumeItem(item.uuid, HookPlayer.Instance.gameObject);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool BindItem(Item item)
        {
            if (item == null) return false;
            if ((item.itemTypes & acceptItemTypes) == 0) return false;
            if (InventoryManager.Instance.GetInventoryAmountOfItem(item.uuid) == 0) return false;

            this.item = item;
            UpdateUI();

            InventoryManager.Instance.eventChangePlayerInventory.AddListener(UpdateUI);
            return true;
        }

        public void UnbindItem()
        {
            item = null;

            OnPointerExit(null);
            UpdateUI();

            InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(UpdateUI);
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void UpdateUI()
        {
            if (item != null &&
                InventoryManager.Instance.GetInventoryAmountOfItem(item.uuid) == 0)
            {
                UnbindItem();
                return;
            }

            if (itemImage != null)
                itemImage.overrideSprite = item != null
                    ? item.sprite
                    : null;

            if (itemText != null)
                itemText.text = item != null
                    ? item.itemName.GetText()
                    : string.Empty;

            if (itemDescription != null)
                itemDescription.text = item != null
                    ? item.itemDescription.GetText()
                    : string.Empty;
        }

        public void OnDrop(Item item)
        {
            BindItem(item);
        }

        protected void OnPointerEnter(BaseEventData eventData)
        {
            if (item == null) return;
            if (eventOnHoverEnter != null) eventOnHoverEnter.Invoke();
        }

        protected void OnPointerExit(BaseEventData eventData)
        {
            if (eventOnHoverExit != null) eventOnHoverExit.Invoke();
        }
    }
}