using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimatorManger : MonoBehaviour
    {
        private Animator animator;

        private static int _moveParameter = Animator.StringToHash("Move");
        private static int _deathTriggerParameter = Animator.StringToHash("DeadTrigger");
        private static int _deathParameter = Animator.StringToHash("Dead");

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimateCharacterMovement(bool isMoving, Vector3 motion)
        {
            if (animator == null) return;
            animator.SetFloat(_moveParameter, isMoving
                ? Mathf.Lerp(animator.GetFloat(_moveParameter), motion.sqrMagnitude, 10 * Time.deltaTime)
                : Mathf.Lerp(animator.GetFloat(_moveParameter), 0, 10 * Time.deltaTime));
        }

        public void AnimateCharacterDeath()
        {
            if (animator == null) return;
            animator.SetTrigger(_deathTriggerParameter);
            animator.SetInteger(_deathParameter, Random.Range(1, 5));
        }
    }
}