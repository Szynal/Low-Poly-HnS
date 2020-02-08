using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Animator))]

    public class CharacterAnimatorManger : MonoBehaviour
    {
        private static int _moveParameter = Animator.StringToHash("move");
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimateCharacterMovement(bool isMoving, Vector3 motion)
        {
            if (animator != null)
            {
                animator.SetFloat("move", isMoving
                    ? Mathf.Lerp(animator.GetFloat(_moveParameter), motion.sqrMagnitude, 10 * Time.deltaTime)
                    : Mathf.Lerp(animator.GetFloat(_moveParameter), 0, 10 * Time.deltaTime));

            }
        }
    }
}