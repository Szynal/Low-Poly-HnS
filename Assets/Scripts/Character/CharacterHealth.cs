using LowPolyHnS;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HealthBar sliderScript = null;
    [SerializeField] private int health;

    private CharacterMovement characterMovement;
    private CharacterAnimatorManger animatorManger;

    private bool isDead;

    public void Start()
    {
        sliderScript.UpdateHealthBar(health);
        characterMovement = GetComponent<CharacterMovement>();
        animatorManger = GetComponent<CharacterAnimatorManger>();
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
        if (isDead) return;
        if (characterMovement != null) characterMovement.EnableRagdoll(1);
        if (animatorManger != null) animatorManger.AnimateCharacterDeath();

        isDead = true;
    }
}