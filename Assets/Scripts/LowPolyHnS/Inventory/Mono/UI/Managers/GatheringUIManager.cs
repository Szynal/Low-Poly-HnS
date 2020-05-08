using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Inventory
{
    public class GatheringUIManager : MonoBehaviour
    {
        private const int TIME_LAYER = 200;
        private static GatheringUIManager Instance;
        private static DatabaseInventory DATABASE_INVENTORY;
        private const string DEFAULT_UI_PATH = "Assets/Content/Prefabs/UI/PlayerUI";

        #region PROPERTIES

        [Space] public Image floatingItem;

        private CanvasScaler canvasScaler;
        private RectTransform floatingItemRT;
        private Animator inventoryAnimator;
        private GameObject inventoryRoot;
        private bool isOpen;

        #endregion

        private void Awake()
        {
            Instance = this;
            if (transform.childCount >= 1)
            {
                inventoryRoot = transform.GetChild(0).gameObject;
                inventoryAnimator = inventoryRoot.GetComponent<Animator>();
            }

            if (floatingItem != null) floatingItemRT = floatingItem.GetComponent<RectTransform>();
            OnDragItem(null, false);
        }

        public void Open()
        {
            if (isOpen) return;

            ChangeState(true);
            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(0f, TIME_LAYER);
            }
        }

        public void Close()
        {
            if (!isOpen) return;

            if (DATABASE_INVENTORY.inventorySettings.pauseTimeOnUI)
            {
                TimeManager.Instance.SetTimeScale(1f, TIME_LAYER);
            }

            ChangeState(false);
        }

        public bool IsOpen()
        {
            return isOpen;
        }

        public static void OpenCloseInventory()
        {
            if (IsInventoryOpen())
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        public static void OpenInventory()
        {
            if (IsInventoryOpen()) return;

            RequireInstance();
            Instance.Open();
        }

        public static void CloseInventory()
        {
            if (!IsInventoryOpen()) return;

            RequireInstance();
            Instance.Close();
        }

        public static bool IsInventoryOpen()
        {
            return Instance != null && Instance.IsOpen();
        }

        private static void RequireInstance()
        {
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();

            if (Instance != null)
            {
                return;
            }

            EventSystemManager.Instance.Wakeup();
            if (DATABASE_INVENTORY.inventorySettings == null)
            {
                Debug.LogError("No inventory database found");
                return;
            }

            GameObject prefab = DATABASE_INVENTORY.inventorySettings.inventoryUIPrefab;
            if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        public static void OnDragItem(Sprite sprite, bool dragging)
        {
            if (!Instance.floatingItem) return;

            Instance.floatingItem.gameObject.SetActive(dragging);
            if (!dragging) return;

            Instance.floatingItem.sprite = sprite;

            Vector2 position = Instance.GetPointerPositionUnscaled(Input.mousePosition);
            Instance.floatingItemRT.anchoredPosition = position;
        }

        private void ChangeState(bool toOpen)
        {
            if (inventoryRoot == null)
            {
                Debug.LogError("Unable to find inventoryRoot");
                return;
            }

            isOpen = toOpen;

            if (inventoryAnimator == null)
            {
                inventoryRoot.SetActive(toOpen);
                return;
            }

            inventoryAnimator.SetBool("State", toOpen);
            InventoryManager.Instance.eventInventoryUI.Invoke(toOpen);
        }

        private Vector2 GetPointerPositionUnscaled(Vector2 mousePosition)
        {
            if (canvasScaler == null)
            {
                canvasScaler = transform.GetComponentInParent<CanvasScaler>();
            }

            if (canvasScaler == null)
            {
                return mousePosition;
            }

            Vector2 referenceResolution = canvasScaler.referenceResolution;
            Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

            float widthRatio = currentResolution.x / referenceResolution.x;
            float heightRatio = currentResolution.y / referenceResolution.y;
            float ratio = Mathf.Lerp(widthRatio, heightRatio, canvasScaler.matchWidthOrHeight);

            return mousePosition / ratio;
        }
    }
}