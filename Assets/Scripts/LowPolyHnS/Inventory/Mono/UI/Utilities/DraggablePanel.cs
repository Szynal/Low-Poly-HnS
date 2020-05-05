using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class DraggablePanel : MonoBehaviour
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public RectTransform targetPanel;
        public bool constraintWithinScreen = true;

        private CanvasScaler canvasScaler;

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Awake()
        {
            SetupEvents(EventTriggerType.BeginDrag, OnDragBegin);
            SetupEvents(EventTriggerType.EndDrag, OnDragEnd);
            SetupEvents(EventTriggerType.Drag, OnDragMove);
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnDragBegin(BaseEventData eventData)
        {
            MovePanel(eventData);
        }

        private void OnDragEnd(BaseEventData eventData)
        {
            MovePanel(eventData);
        }

        private void OnDragMove(BaseEventData eventData)
        {
            MovePanel(eventData);
        }

        private void MovePanel(BaseEventData eventData)
        {
            PointerEventData pointerData = (PointerEventData) eventData;
            if (pointerData == null) return;

            Vector2 currentPosition = targetPanel.anchoredPosition;
            Vector2 deltaUnscaled = GetPonterDataDeltaUnscaled(pointerData.delta);

            currentPosition.x += deltaUnscaled.x;
            currentPosition.y += deltaUnscaled.y;

            if (constraintWithinScreen)
            {
                Vector3 pos = targetPanel.localPosition;
                RectTransform parentPanel = (RectTransform) targetPanel.parent;

                Vector3 minPosition = parentPanel.rect.min - targetPanel.rect.min;
                Vector3 maxPosition = parentPanel.rect.max - targetPanel.rect.max;

                currentPosition = new Vector2(
                    Mathf.Clamp(currentPosition.x, minPosition.x, maxPosition.x),
                    Mathf.Clamp(currentPosition.y, minPosition.y, maxPosition.y)
                );
            }

            targetPanel.anchoredPosition = currentPosition;
        }

        private Vector2 GetPonterDataDeltaUnscaled(Vector2 delta)
        {
            if (canvasScaler == null) canvasScaler = transform.GetComponentInParent<CanvasScaler>();
            if (canvasScaler == null) return delta;

            Vector2 referenceResolution = canvasScaler.referenceResolution;
            Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

            float widthRatio = currentResolution.x / referenceResolution.x;
            float heightRatio = currentResolution.y / referenceResolution.y;
            float ratio = Mathf.Lerp(widthRatio, heightRatio, canvasScaler.matchWidthOrHeight);

            return delta / ratio;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

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