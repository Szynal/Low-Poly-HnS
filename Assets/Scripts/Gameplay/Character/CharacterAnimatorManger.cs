using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimatorManger : MonoBehaviour
    {
        private Animator animator;

        private static int MOVE_PARAMETER = Animator.StringToHash("Move");
        private static int DEATH_TRIGGER_PARAMETER = Animator.StringToHash("DeadTrigger");
        private static int DEATH_PARAMETER = Animator.StringToHash("Dead");
        private static int HIT_PARAMETER = Animator.StringToHash("Hit");

        [SerializeField] private GameObject rippleClickEffect = null;
        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimateCharacterMovement(bool isMoving, Vector3 motion)
        {
            if (animator == null) return;
            animator.SetFloat(MOVE_PARAMETER, isMoving
                ? Mathf.Lerp(animator.GetFloat(MOVE_PARAMETER), motion.sqrMagnitude, 10 * Time.deltaTime)
                : Mathf.Lerp(animator.GetFloat(MOVE_PARAMETER), 0, 2 * Time.deltaTime));

            if (animator.GetFloat(MOVE_PARAMETER) < 0.1)
            {
                rippleClickEffect.SetActive(false);
            }
        }

        public void AnimateCharacterDeath()
        {
            if (animator == null) return;
            animator.SetTrigger(DEATH_TRIGGER_PARAMETER);
            animator.SetInteger(DEATH_PARAMETER, Random.Range(1, 5));
        }

        public void AnimateCharacterHit()
        {
            if (animator == null) return;
            animator.SetTrigger(HIT_PARAMETER);
        }

        public bool GetMoveParam()
        {
            return animator.GetFloat(MOVE_PARAMETER) > 0;
        }
    }
}