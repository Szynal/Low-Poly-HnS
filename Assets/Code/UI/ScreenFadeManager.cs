using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage = null;

    private Coroutine fadeCoroutine;


    //METALLICA !!!
    public void FadeToBlack(bool isBlackening, float fadeTime, Action callbackFunction = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(fadeToBlack(isBlackening, fadeTime, callbackFunction));
    }

    public void StopFading()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }

    private IEnumerator fadeToBlack(bool isBlackening, float fadeTime, Action callbackFunction = null)
    {
        //Preparing the lerp
        fadeImage.enabled = true;
        float lerpTime = 0f;
        float targetOpacity;
        float startOpacity = fadeImage.color.a;
        targetOpacity = isBlackening ? 1f : 0f;
        float lerpRate = 1f / fadeTime;

        //Lerp
        while (fadeImage.color.a != targetOpacity)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b,
                Mathf.Lerp(startOpacity, targetOpacity, lerpTime));
            lerpTime += Time.unscaledDeltaTime * lerpRate;

            if (Mathf.Abs(fadeImage.color.a - targetOpacity) <= 0.05f)
            {
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetOpacity);
            }

            yield return null;
        }

        callbackFunction?.Invoke();

        if (isBlackening == false)
        {
            fadeImage.enabled = false;
        }
    }

    public void UnfadeFromBlack(float blackscreenTime, float fadeTime, Action callbackFunction = null,
        bool forceCameraToChange = false)
    {
        StartCoroutine(unfadeFromBlack(blackscreenTime, fadeTime, callbackFunction, forceCameraToChange));
    }

    private IEnumerator unfadeFromBlack(float blackscreenTime, float fadeTime, Action callbackFunction,
        bool forceCameraToChange)
    {
        if (forceCameraToChange)
        {
            Time.timeScale = 1f;
        }

        yield return new WaitForSecondsRealtime(blackscreenTime);

        yield return StartCoroutine(fadeToBlack(false, fadeTime, callbackFunction));

        FE_PlayerInventoryInteraction.Instance.InputController.ChangeInputMode(EInputMode.Full);
    }
}