using UnityEngine;

namespace LowPolyHnS
{
    public abstract class Health : MonoBehaviour
    {
        [Header("Inherited from Health")] [SerializeField]
        protected int HealthMax = 100;

        public int HealthCurrent;
        public bool IsInvulnerable = false;
        public bool IgnoreDamageZones = false;

        protected virtual void Awake()
        {
            if (HealthCurrent <= 0 || HealthCurrent > HealthMax)
            {
                HealthCurrent = HealthMax;
            }
        }

        public virtual void TakeDamage(int dmgAmount)
        {
            if (IsInvulnerable)
            {
                return;
            }

            HealthCurrent -= dmgAmount;

            if (HealthCurrent <= 0)
            {
                Die();
            }
        }

        public int GetMaxHealth()
        {
            return HealthMax;
        }

        protected virtual void Die()
        {
        }
    }
}