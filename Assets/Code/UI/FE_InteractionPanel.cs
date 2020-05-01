using System.Collections;
using UnityEngine;

public class FE_InteractionPanel : MonoBehaviour
{
    [SerializeField] private float showAnimTime = 0.8f;
    private float basePos;
    private RectTransform rectTransform;
    private Coroutine animCoroutine;
    private bool isVisible;

    public bool GetVisibility()
    {
        return isVisible;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("InventoryPanel was added to an object without a RectTransform! Obj: " + name);
        }
    }

    private void Start()
    {
        basePos = 0f - GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.height / 2f;
    }

    public void Appear()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }

        animCoroutine = StartCoroutine(animatePanel(true, showAnimTime));
        isVisible = true;
    }

    public void Disappear()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            animCoroutine = StartCoroutine(animatePanel(false, showAnimTime));
        }
        else
        {
            rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x,
                basePos - rectTransform.rect.height / 2f, 0f);
        }

        isVisible = false;
    }

    protected IEnumerator animatePanel(bool _isAppearing, float _time)
    {
        float _desiredY = basePos + rectTransform.rect.height / 2f;

        if (_isAppearing == false)
        {
            _desiredY = basePos - rectTransform.rect.height / 2f;
        }

        float _lerpRate = 1f / showAnimTime;
        float _startyY = rectTransform.anchoredPosition.y;

        while (_time >= 0f)
        {
            _time -= Time.unscaledDeltaTime * _lerpRate;

            rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x,
                Mathf.Lerp(_startyY, _desiredY, 1f - _time), 0f); //1 - time, bo zmniejszamy czas zamiast zwiekszac

            if (Mathf.Abs(rectTransform.anchoredPosition.y - _desiredY) <= 1f)
            {
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, _desiredY, 0f);

                yield break;
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        animCoroutine = null;
        StopAllCoroutines();
    }
}