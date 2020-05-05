using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.UI
{
    [RequireComponent(typeof(Animation))]
    public class LoadingCircle : MonoBehaviour
    {
        public Animation CircleRotate = null;
        public Image[] ImagesToFade = null;
        public float FadeTime = 1f;

        private bool isDisabling;
        private float fadeProgress;

        private void OnEnable()
        {
            foreach (Image image in ImagesToFade)
            {
                image.color = getFullColor(image.color);
            }

            if (CircleRotate != null)
            {
                CircleRotate.Play();
            }
        }

        private void OnDisable()
        {
            isDisabling = false;
        }

        private void Update()
        {
            if (isDisabling == false)
            {
                return;
            }

            fadeProgress += Time.unscaledDeltaTime / FadeTime;

            foreach (Image image in ImagesToFade)
            {
                image.color = Color.Lerp(getFullColor(image.color),
                    getFadedColor(image.color), fadeProgress);
            }

            if (fadeProgress >= 1f)
            {
                gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            fadeProgress = 0f;
            isDisabling = true;
        }

        private Color getFullColor(Color color)
        {
            return new Color(color.r, color.g, color.b, 1f);
        }

        private Color getFadedColor(Color input)
        {
            return new Color(input.r, input.g, input.b, 0f);
        }
    }
}