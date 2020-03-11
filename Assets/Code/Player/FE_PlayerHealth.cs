using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;

[System.Serializable]
public class FE_PlayerHealth : FE_Health
{
    [SerializeField] FE_DeathCutscene deathCutscene = null;

    private List<FE_DangerZone> dangerZones = new List<FE_DangerZone>();
    private FE_UIController uiController;

    public delegate void PlayerHealthChangeDelegate(int _newHealth);
    public PlayerHealthChangeDelegate OnPlayerHealthChange;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        FE_UIController.Instance?.SetHealthColor(HealthCurrent);
    }

    public override void TakeDamage(int _dmgAmount, Transform _attacker, bool _stealthAttack = false, bool _forceDamage = false)
    {
        if (GameManager.Instance.CheatOptions.GodMode == true)
        {
            return;
        }

        if (IsInvulnerable == false || _forceDamage == false)
        {
            HealthCurrent -= _dmgAmount;

            if (HealthCurrent <= 0)
            {
                FE_SpecialDeathCutsceneStarter _specialCutscene = _attacker.GetComponent<FE_SpecialDeathCutsceneStarter>();
                if (_specialCutscene != null)
                {
                    _specialCutscene.StartTargetCutscene();
                    this.WaitFramesAndCall(1, die, _stealthAttack);
                }
                else
                {
                    die(_stealthAttack);
                }
            }

            OnPlayerHealthChange?.Invoke(HealthCurrent);
        }
    }

    protected override void die(bool _silentDeath)
    {
        base.die(_silentDeath);

        deathCutscene.PlayCutscene();
    }

    public void HealFromBackstab()
    {
        HealthCurrent += 10;
        HealthCurrent = Mathf.Clamp(HealthCurrent, 0, healthMax);

        OnPlayerHealthChange?.Invoke(HealthCurrent);
    }

    public void HealToFull()
    {
        HealthCurrent = healthMax;

        OnPlayerHealthChange?.Invoke(HealthCurrent);
    }

    public void AddDanger(FE_DangerZone _newDanger)
    {
        if(dangerZones.Contains(_newDanger) == false)
        {
            dangerZones.Add(_newDanger);

            if(FE_UIController.Instance == null)
            {
                return;
            }

            FE_UIController.Instance.ShowHealth(true, true);
        }
    }

    public void RemoveDanger(FE_DangerZone _removeDanger)
    {
        if(dangerZones.Contains(_removeDanger) == true)
        {
            dangerZones.Remove(_removeDanger);
            if(dangerZones.Count <= 0)
            {
                if(FE_UIController.Instance == null)
                {
                    return;
                }

                FE_UIController.Instance.ShowHealth(false, false);
            }
        }
    }
}