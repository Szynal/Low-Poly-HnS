using LowPolyHnS;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HealthBar sliderScript = null;
    [SerializeField] private int health;
    
    public void Awake()
    {
        sliderScript.UpdateHealthBar(health);
    }

    public void CharacterTakeDamage(int damage)
    {
        health -= damage;
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
        //TODO
        Debug.Log("Death");
    }
}