using System;
using LowPolyHnS;
using UnityEngine;

[Serializable]
public class CharacterHealth : Health
{
    [Header("CharacterHealth")] [SerializeField]
    private DeathCutscene deathCutscene = null;

    [SerializeField] private HealthBar sliderScript = null;

    public delegate void PlayerHealthChangeDelegate(int newHealth);

    public PlayerHealthChangeDelegate OnPlayerHealthChange;

    private void Start()
    {
        if (sliderScript != null)
        {
            sliderScript.UpdateHealthBar(HealthCurrent);
        }
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

        if (HealthCurrent <= 0)
        {
            Die();
        }

        OnPlayerHealthChange?.Invoke(HealthCurrent);
    }

    protected override void Die()
    {
        base.Die();
        deathCutscene.PlayCutscene();
    }
}