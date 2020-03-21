using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS
{
    public class HealthBar : MonoBehaviour
    {
        private Slider healthSlider;
        [SerializeField] private MiniHealthBar miniHealthBar = null;
        public int Health { get; private set; }

        public void Start()
        {
            healthSlider = transform.GetComponent<Slider>();
        }

        public void UpdateHealthBar(int hp)
        {
            Health = hp;
            if (healthSlider != null) healthSlider.value = hp;
            if (miniHealthBar != null) miniHealthBar.GetComponent<Slider>().value = hp;
        }
    }
}