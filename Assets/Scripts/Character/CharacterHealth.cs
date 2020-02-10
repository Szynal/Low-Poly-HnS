using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]private int health;
    [SerializeField] private HealthBar sliderScript;

    public void Awake()
    {
        sliderScript.UpdateHealthBar(health);
    }

    public void CharacterTakeDMG(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            sliderScript.UpdateHealthBar(0);
            Death();
            return;
        }
        sliderScript.UpdateHealthBar(health);
            
    }

    public void Death()
    {
        Debug.Log("Death");
    }

}
