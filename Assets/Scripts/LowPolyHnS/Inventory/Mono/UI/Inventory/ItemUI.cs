using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("LowPolyHnS/UI/Item")]
    public class ItemUI : MonoBehaviour
    {
        protected static DatabaseInventory DATABASE_INVENTORY;

        // PROPERTIES: ----------------------------------------------------------------------------

        [HideInInspector] public Item item { get; private set; }

        protected Button button;

        public Image image;
        public Graphic color;
        public Text textName;
        public Text textDescription;

        [Space] public GameObject containerAmount;
        public Text textAmount;

        [Space] public GameObject equipped;

        [Space] public UnityEvent eventOnHoverEnter;
        public UnityEvent eventOnHoverExit;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public virtual void Setup(Item item, int amount)
        {
            UpdateUI(item, amount);
            button = gameObject.GetComponentInChildren<Button>();

            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            if (DATABASE_INVENTORY.inventorySettings.onDragGrabItem)
            {
                SetupEvents(EventTriggerType.BeginDrag, OnDragBegin);
                SetupEvents(EventTriggerType.EndDrag, OnDragEnd);
                SetupEvents(EventTriggerType.Drag, OnDragMove);
            }

            SetupEvents(EventTriggerType.PointerEnter, OnPointerEnter);
            SetupEvents(EventTriggerType.PointerExit, OnPointerExit);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void UpdateUI(Item item, int amount)
        {
            this.item = item;

            if (image != null && item.sprite != null) image.sprite = item.sprite;
            if (color != null) color.color = item.itemColor;

            if (textName != null) textName.text = item.itemName.GetText();
            if (textDescription != null) textDescription.text = item.itemDescription.GetText();
            if (textAmount != null)
            {
                textAmount.text = amount.ToString();
                if (containerAmount != null)
                {
                    containerAmount.SetActive(amount != 1);
                }
            }

            if (equipped != null)
            {
                int equippedAmount = InventoryManager.Instance.HasEquiped(
                    HookPlayer.Instance == null ? null : HookPlayer.Instance.gameObject,
                    this.item.uuid
                );

                equipped.SetActive(equippedAmount > 0);
            }
        }

        public virtual void OnClick()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            InventoryManager.Instance.ConsumeItem(item.uuid);
        }

        public virtual void OnDragBegin(BaseEventData eventData)
        {
            if (DATABASE_INVENTORY != null && DATABASE_INVENTORY.inventorySettings.cursorDrag != null)
            {
                Cursor.SetCursor(
                    DATABASE_INVENTORY.inventorySettings.cursorDrag,
                    DATABASE_INVENTORY.inventorySettings.cursorDragHotspot,
                    CursorMode.Auto
                );
            }

            if (DATABASE_INVENTORY != null &&
                DATABASE_INVENTORY.inventorySettings.onDragGrabItem && item.sprite != null)
            {
                InventoryUIManager.OnDragItem(item.sprite, true);
            }

            eventData.Use();
        }

        public virtual void OnDragEnd(BaseEventData eventData)
        {
            if (DATABASE_INVENTORY != null && DATABASE_INVENTORY.inventorySettings.cursorDrag != null)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }

            InventoryUIManager.OnDragItem(null, false);
            PointerEventData pointerData = (PointerEventData) eventData;
            if (pointerData == null) return;
            if (pointerData.pointerCurrentRaycast.gameObject == null) return;

            GameObject target = pointerData.pointerCurrentRaycast.gameObject;
            IgniterDropToConsume[] targetsDrop = target.GetComponents<IgniterDropToConsume>();
            EquipSlotUI[] targetsEquipSlotUI = target.GetComponents<EquipSlotUI>();
            HotbarUI[] targetsHotbarUI = target.GetComponents<HotbarUI>();
            ItemUI targetItemUI = target.GetComponent<ItemUI>();

            if (targetsDrop.Length > 0)
            {
                for (int i = 0; i < targetsDrop.Length; ++i)
                {
                    targetsDrop[i].OnDrop(item);
                    eventData.Use();
                }
            }
            else if (targetsEquipSlotUI.Length > 0)
            {
                for (int i = 0; i < targetsEquipSlotUI.Length; ++i)
                {
                    targetsEquipSlotUI[i].OnDrop(item);
                    eventData.Use();
                }
            }
            else if (targetsHotbarUI.Length > 0)
            {
                for (int i = 0; i < targetsHotbarUI.Length; ++i)
                {
                    targetsHotbarUI[i].OnDrop(item);
                    eventData.Use();
                }
            }
            else if (targetItemUI == null)
            {
                bool mouseOverUI = EventSystemManager.Instance.IsPointerOverUI();
                if (!mouseOverUI && DATABASE_INVENTORY.inventorySettings.canDropItems && item.prefab != null)
                {
                    Vector3 position = pointerData.pointerCurrentRaycast.worldPosition + Vector3.up * 0.1f;
                    Vector3 direction = position - HookPlayer.Instance.transform.position;

                    if (direction.magnitude > DATABASE_INVENTORY.inventorySettings.dropItemMaxDistance)
                    {
                        position = HookPlayer.Instance.transform.position +
                                   direction.normalized * DATABASE_INVENTORY.inventorySettings.dropItemMaxDistance;
                    }

                    Instantiate(item.prefab, position, Quaternion.identity);
                    InventoryManager.Instance.SubstractItemFromInventory(item.uuid);
                }

                eventData.Use();
            }
            else if (item.uuid != targetItemUI.item.uuid)
            {
                Button otherButton = pointerData.pointerCurrentRaycast.gameObject.GetComponentInChildren<Button>();
                if (otherButton != null)
                {
                    HighlightButton(button, false);
                    HighlightButton(otherButton, false);
                }

                InventoryManager.Instance.UseRecipe(item.uuid, targetItemUI.item.uuid);
                eventData.Use();
            }
        }

        public virtual void OnDragMove(BaseEventData eventData)
        {
            PointerEventData pointerData = (PointerEventData) eventData;
            if (pointerData == null) return;

            if (DATABASE_INVENTORY != null &&
                DATABASE_INVENTORY.inventorySettings.onDragGrabItem && item.sprite != null)
            {
                InventoryUIManager.OnDragItem(item.sprite, true);
            }

            if (pointerData.pointerCurrentRaycast.gameObject == null) return;
            GameObject target = pointerData.pointerCurrentRaycast.gameObject;
            ItemUI otherItemUI = target.GetComponent<ItemUI>();

            if (otherItemUI != null)
            {
                Button otherButton = pointerData.pointerCurrentRaycast.gameObject.GetComponentInChildren<Button>();
                if (otherButton != null)
                {
                    HighlightButton(button, true);
                    HighlightButton(otherButton, true);
                }

                eventData.Use();
            }
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void SetupEvents(EventTriggerType eventType, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();
            eventTriggerEntry.eventID = eventType;
            eventTriggerEntry.callback.AddListener(callback);

            EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = gameObject.AddComponent<EventTrigger>();

            eventTrigger.triggers.Add(eventTriggerEntry);
        }

        protected void HighlightButton(Button button, bool highlight)
        {
            switch (button.transition)
            {
                case Selectable.Transition.ColorTint:
                    button.image.color = highlight ? button.colors.pressedColor : Color.white;
                    break;

                case Selectable.Transition.Animation:
                    button.animator.SetTrigger(highlight
                        ? button.animationTriggers.pressedTrigger
                        : button.animationTriggers.normalTrigger
                    );
                    break;

                case Selectable.Transition.SpriteSwap:
                    button.image.overrideSprite = highlight
                        ? button.spriteState.pressedSprite
                        : button.image.sprite;
                    break;
            }
        }

        protected void OnPointerEnter(BaseEventData eventData)
        {
            if (eventOnHoverEnter != null) eventOnHoverEnter.Invoke();
        }

        protected void OnPointerExit(BaseEventData eventData)
        {
            if (eventOnHoverExit != null) eventOnHoverExit.Invoke();
        }
    }
}