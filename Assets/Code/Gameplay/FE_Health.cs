using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FE_Health : MonoBehaviour
{
    [Header("Inherited from FE_Health")]
    [SerializeField] protected int healthMax = 100;
    public int HealthCurrent = 0;
    public bool IsInvulnerable = false;
    public bool IgnoreDamageZones = false;

    protected virtual void Awake()
    {
        if (HealthCurrent <= 0 || HealthCurrent > healthMax)
        {
            Debug.Log(gameObject.name + " has current health set up badly. Setting to maximum health value.", this);
            HealthCurrent = healthMax;
        }
    }

    public virtual void TakeDamage(int _dmgAmount, Transform _attacker, bool _stealthAttack = false, bool _forceDamage = false)
    {
        if (IsInvulnerable == false || _forceDamage == true)
        {
            HealthCurrent -= _dmgAmount;

            if (HealthCurrent <= 0)
            {
                die(_stealthAttack);
            }
        }
    }

    public int GetMaxHealth()
    {
        return healthMax;
    }

    protected virtual void die(bool _silentDeath)
    {
        //play death anim
    }
}
