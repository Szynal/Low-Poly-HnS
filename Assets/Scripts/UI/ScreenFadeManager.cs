using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    public class ScreenFadeManager : MonoBehaviour
    {
        [SerializeField] private Image fadeImage = null;

        private Coroutine fade;

        public void FadeToBlack(bool isBlackening, float fadeTime, Action callbackFunction = null)
        {
            if (fade != null)
            {
                StopCoroutine(fade);
            }

            fade = StartCoroutine(fadeToBlack(isBlackening, fadeTime, callbackFunction));
        }

        public void StopFading()
        {
            if (fade != null)
            {
                StopCoroutine(fade);
            }
        }

        private IEnumerator fadeToBlack(bool isBlackening, float fadeTime, Action callbackFunction = null)
        {
            fadeImage.enabled = true;
            float lerpTime = 0f;
            float startOpacity = fadeImage.color.a;
            float targetOpacity = isBlackening ? 1f : 0f;
            float lerpRate = 1f / fadeTime;

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
        }
    }
}