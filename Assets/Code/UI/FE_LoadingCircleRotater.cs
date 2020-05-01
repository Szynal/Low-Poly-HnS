using UnityEngine;
using UnityEngine.UI;

public class FE_LoadingCircleRotater : MonoBehaviour
{
    [SerializeField] private Transform transformToRotate = null;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Image[] imagesToFade = null;
    [SerializeField] private float fadeTime = 1f;

    private bool isDisabling;
    private float fadeProgress;

    private void OnEnable()
    {
        for (int i = 0; i < imagesToFade.Length; i++)
        {
            imagesToFade[i].color = getFullColor(imagesToFade[i].color);
        }

        transformToRotate.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Update()
    {
        transformToRotate.Rotate(0f, 0f, rotationSpeed * Time.unscaledDeltaTime);

        if (isDisabling)
        {
            fadeProgress += Time.unscaledDeltaTime / fadeTime;

            for (int i = 0; i < imagesToFade.Length; i++)
            {
                imagesToFade[i].color = Color.Lerp(getFullColor(imagesToFade[i].color),
                    getFadedColor(imagesToFade[i].color), fadeProgress);
            }

            if (fadeProgress >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        isDisabling = false;
    }

    public void Hide()
    {
        fadeProgress = 0f;
        isDisabling = true;
    }

    private Color getFullColor(Color _input)
    {
        return new Color(_input.r, _input.g, _input.b, 1f);
    }

    private Color getFadedColor(Color _input)
    {
        return new Color(_input.r, _input.g, _input.b, 0f);
    }
}