using System.Collections;
using UnityEngine;

public class FE_InventoryPanel : MonoBehaviour
{
    [SerializeField] private float upDownInputDeadzone = 0.5f;
    [Space(5f)] [SerializeField] private FE_ItemWheel weaponInventoryWheel = null;
    [SerializeField] private FE_ItemWheel useableInventoryWheel = null;
    [Space(5f)] [SerializeField] private float menuMovementAnimTime = 1f;

    private float inputDelayRemaining;
    private FE_ItemWheel currentItemWheel;
    private RectTransform rectTransform;
    private FE_PlayerInputController inputScript;
    private FE_UIController masterScript;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        masterScript = FE_UIController.Instance;
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        currentItemWheel = null;

        useableInventoryWheel.Initialize(FE_PlayerInventoryInteraction.Instance.GetUseableInventory(), null);

        if (inputScript == null)
        {
            inputScript = FE_PlayerInputController.Instance;
        }

        inputScript.InventoryUpDownInput += handleUpDownInput;
        //TODO: create an indication to show that given inventory is empty
    }

    private void OnDisable()
    {
        inputScript.InventoryUpDownInput -= handleUpDownInput;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseCurrentlySelected();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            masterScript.CloseInventory();
        }
    }

    private void handleUpDownInput(float _value)
    {
        if (inputDelayRemaining > 0f || Mathf.Abs(_value) < upDownInputDeadzone)
        {
            return;
        }

        if (_value > 0f && currentItemWheel != weaponInventoryWheel)
        {
            if (weaponInventoryWheel.CanBeOpened)
            {
                StartCoroutine(animateMenuMovement(-rectTransform.rect.height / 4f,
                    menuMovementAnimTime * (currentItemWheel == null ? 1f : 2f)));
                currentItemWheel = weaponInventoryWheel;
                inputDelayRemaining = menuMovementAnimTime + 0.1f;
            }
            else if (currentItemWheel != null)
            {
                StartCoroutine(animateMenuMovement(0f, menuMovementAnimTime * 1f));
                currentItemWheel = null;
                inputDelayRemaining = menuMovementAnimTime + 0.1f;
            }
        }
        else if (_value < 0f && currentItemWheel != useableInventoryWheel)
        {
            if (useableInventoryWheel.CanBeOpened)
            {
                StartCoroutine(animateMenuMovement(rectTransform.rect.height / 4f,
                    menuMovementAnimTime * (currentItemWheel == null ? 1f : 2f)));
                currentItemWheel = useableInventoryWheel;
                inputDelayRemaining = menuMovementAnimTime + 0.1f;
            }
            else if (currentItemWheel != null)
            {
                StartCoroutine(animateMenuMovement(0f, menuMovementAnimTime * 1f));
                currentItemWheel = null;
                inputDelayRemaining = menuMovementAnimTime + 0.1f;
            }
        }
    }


    public void UseCurrentlySelected()
    {
        if (currentItemWheel == null)
        {
            return;
        }

        currentItemWheel.UseCurrent();
    }

    private IEnumerator animateMenuMovement(float _targetY, float _animTime)
    {
        Vector3 _startPos = transform.localPosition;
        Vector3 _targetPos = new Vector3(_startPos.x, _targetY, _startPos.z);

        float _lerpProgress = 0f; //TODO: animation curve for non-linear progress?

        while (_lerpProgress < 1f)
        {
            _lerpProgress += Time.unscaledDeltaTime / _animTime;
            transform.localPosition = Vector3.Lerp(_startPos, _targetPos, _lerpProgress);

            yield return null;
        }
    }
}