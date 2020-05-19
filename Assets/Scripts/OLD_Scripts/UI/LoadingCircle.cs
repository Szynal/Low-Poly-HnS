using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    [RequireComponent(typeof(Animation))]
    public class LoadingCircle : MonoBehaviour
    {
        [SerializeField] private Transform transformToRotate = null;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Image[] imagesToFade = null;
        [SerializeField] private float fadeTime = 1f;

        private bool isDisabling;
        private float fadeProgress;

        private void OnEnable()
        {
            foreach (Image image in imagesToFade)
            {
                image.color = GetFullColor(image.color);
            }

            transformToRotate.localRotation = Quaternion.Euler(Vector3.zero);
        }

        private void Update()
        {
            transformToRotate.Rotate(0f, 0f, rotationSpeed * Time.unscaledDeltaTime);

            if (!isDisabling)
            {
                return;
            }

            fadeProgress += Time.unscaledDeltaTime / fadeTime;

            foreach (Image image in imagesToFade)
            {
                image.color = Color.Lerp(GetFullColor(image.color), GetFadedColor(image.color),
                    fadeProgress);
            }

            if (fadeProgress >= 1f)
            {
                gameObject.SetActive(false);
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

        private static Color GetFullColor(Color input)
        {
            return new Color(input.r, input.g, input.b, 1f);
        }

        private static Color GetFadedColor(Color input)
        {
            return new Color(input.r, input.g, input.b, 0f);
        }
    }
}