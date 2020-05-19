using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    public class LoadingCircle : MonoBehaviour
    {
        public Transform TransformToRotate = null;
        public float RotationSpeed = 90f;
        public Image[] ImagesToFade = null;
        public TextMeshProUGUI Text = null;
        public float FadeTime = 1f;

        private bool isDisabling;
        private float fadeProgress;

        private void OnEnable()
        {
            foreach (Image image in ImagesToFade)
            {
                if (image != null)
                {
                    image.color = GetFullColor(image.color);
                }
            }

            if (Text != null)
            {
                Text.color = GetFullColor(Text.color);
            }

            if (TransformToRotate != null)
            {
                TransformToRotate.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        private void Update()
        {
            if (TransformToRotate != null)
            {
                TransformToRotate.Rotate(0f, 0f, -RotationSpeed * Time.unscaledDeltaTime);
            }

            if (!isDisabling)
            {
                return;
            }

            fadeProgress += Time.unscaledDeltaTime / FadeTime;

            foreach (Image image in ImagesToFade)
            {
                if (image != null)
                {
                    image.color = Color.Lerp(GetFullColor(image.color), GetFadedColor(image.color),
                        fadeProgress);
                }
            }

            if (Text != null)
            {
                Text.color = Color.Lerp(GetFullColor(Text.color), GetFadedColor(Text.color),
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