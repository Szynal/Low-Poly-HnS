using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Animator))]
    public class Trap : MonoBehaviour
    {
        public int DamageDeal = 5;
        public float TimeStep = 0.5f;

        private CharacterHealth characterHealth;
        private bool deadZoneActive;

        private Animator animator;
        private AnimatorStateInfo trapState;
        private static int _stateNameHash = Animator.StringToHash("TrapOpenClose");
        private static int _chestParameter = Animator.StringToHash("Open");
        private bool trapOn;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            ActivateTrap();
        }

        private void OnTriggerStay(Collider other)
        {
            characterHealth = other.GetComponent<CharacterHealth>();
            if (characterHealth == null || deadZoneActive) return;
            TimeDmg();
        }

        private void OnTriggerExit(Collider other)
        {
            DeactivateTrap();
        }

        private void ActivateTrap()
        {
            trapState = animator.GetCurrentAnimatorStateInfo(0);
            trapOn = AnimationTools.PlayAnimation(animator, _stateNameHash, _chestParameter, trapOn,
                trapState.normalizedTime);
        }

        private async void DeactivateTrap()
        {
            await Task.Delay(TimeSpan.FromSeconds(4));
            trapState = animator.GetCurrentAnimatorStateInfo(0);
            trapOn = AnimationTools.PlayAnimation(animator, _stateNameHash, _chestParameter, trapOn,
                trapState.normalizedTime);
        }

        private async void TimeDmg()
        {
            characterHealth.TakeDamage(DamageDeal);
            deadZoneActive = true;
            await Task.Delay(TimeSpan.FromSeconds(TimeStep));
            deadZoneActive = false;
        }
    }
}