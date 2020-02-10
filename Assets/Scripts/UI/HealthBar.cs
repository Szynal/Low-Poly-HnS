using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private Slider HealthSlider;

    public void Start()
    {
        //HealthSlider = transform.GetComponent<Slider>();
    }

    public void UpdateHealthBar(int hp)
    {
        health = hp;
        HealthSlider.value = hp;
    }
}
