using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LowPolyHnS
{
    public class AreaDamage : MonoBehaviour
    {
        private CharacterHealth characterHealth;
        public int DamageDeal = 5;
        private bool deadZoneActive;
        public float TimeStep = 0.5f;

        private void OnTriggerStay(Collider other)
        {
            characterHealth = other.GetComponent<CharacterHealth>();
            if (characterHealth == null || deadZoneActive) return;
            TimeDmg(other);
        }

        private async void TimeDmg(Collider other)
        {
            Debug.Log("dmg");
            characterHealth.CharacterTakeDMG(DamageDeal);
            deadZoneActive = true;
            await Task.Delay(TimeSpan.FromSeconds(TimeStep));
            deadZoneActive = false;
        }
    }
}