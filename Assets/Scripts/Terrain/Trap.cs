using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Collider))]
    public class Trap : MonoBehaviour
    {
        public int DamageDeal = 5;
        public float TimeStep = 0.5f;

        private new Animation animation = null;

        private CharacterHealth characterHealth;
        private bool deadZoneActive;

        private string[] animationState;

        private void Start()
        {
            animation = GetComponent<Animation>();
            Debug.Log(animation);
            if (animation != null) animationState = new string[animation.GetClipCount()];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (animationState != null) animation.Play("an_TrapOn");
        }

        private void OnTriggerStay(Collider other)
        {
            characterHealth = other.GetComponent<CharacterHealth>();
            if (characterHealth == null || deadZoneActive) return;
            TimeDmg();
        }

        private void OnTriggerExit(Collider other)
        {
            if (animationState != null) animation.Play("an_TrapOff");
        }

        private async void TimeDmg()
        {
            characterHealth.CharacterTakeDamage(DamageDeal);
            deadZoneActive = true;
            await Task.Delay(TimeSpan.FromSeconds(TimeStep));
            deadZoneActive = false;
        }
    }
}