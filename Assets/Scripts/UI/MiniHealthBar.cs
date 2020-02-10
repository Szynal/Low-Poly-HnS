using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Slider))]
    public class MiniHealthBar : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition = null;
        [SerializeField] Slider miniHealthSlider = null;
        private Transform miniHealthSliderPos;

        private void Start()
        {
            if (miniHealthSlider != null)
            {
                miniHealthSliderPos = miniHealthSlider.transform;
            }
        }

        private void Update()
        {
            RotateTowardsPlayer();
        }

        private void RotateTowardsPlayer()
        {
            if (cameraPosition == null || miniHealthSlider == null) return;
            miniHealthSliderPos.LookAt(cameraPosition.position);
        }
    }
}