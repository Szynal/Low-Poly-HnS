using System;
using LowPolyHnS;
using UnityEngine;

[Serializable]
public class CharacterHealth : Health
{
    private CharacterMovement characterMovement;
    private CharacterAnimatorManger animatorManger;
    [Header("CharacterHealth")]
    [SerializeField] private DeathCutscene deathCutscene = null;
    [SerializeField] private HealthBar sliderScript = null;

    public delegate void PlayerHealthChangeDelegate(int newHealth);

    public PlayerHealthChangeDelegate OnPlayerHealthChange;

    private void Start()
    {
        if (sliderScript != null)
        {
            sliderScript.UpdateHealthBar(HealthCurrent);
        }

        characterMovement = GetComponent<CharacterMovement>();
        animatorManger = GetComponent<CharacterAnimatorManger>();
    }

    public override void TakeDamage(int dmgAmount)
    {
        if (GameManager.Instance.CheatOptions.GodMode)
        {
            return;
        }

        if (IsInvulnerable)
        {
            return;
        }

        HealthCurrent -= dmgAmount;
        sliderScript.UpdateHealthBar(HealthCurrent);

        if (animatorManger != null)
        {
            animatorManger.AnimateCharacterHit();
        }

        if (HealthCurrent <= 0)
        {
            Die();
        }

        OnPlayerHealthChange?.Invoke(HealthCurrent);
    }

    protected override void Die()
    {
        base.Die();

        if (characterMovement != null)
        {
            characterMovement.EnableRagdoll(1);
        }

        if (animatorManger != null)
        {
            animatorManger.AnimateCharacterDeath();
        }

        deathCutscene.PlayCutscene();
    }
}