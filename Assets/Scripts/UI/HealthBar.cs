using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Slider HealthSlider;
    [SerializeField] private MiniHealthBar miniHealthBar;

    public void Start()
    {
        //HealthSlider = transform.GetComponent<Slider>();
    }

    public void UpdateHealthBar(int hp)
    {
        health = hp;
        HealthSlider.value = hp;
        miniHealthBar.GetComponent<Slider>().value = hp;
    }
}
