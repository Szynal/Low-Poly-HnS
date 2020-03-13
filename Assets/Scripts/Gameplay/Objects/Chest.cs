using UnityEngine;

namespace LowPolyHnS
{
    [RequireComponent(typeof(Animator))]
    public class Chest : MonoBehaviour
    {
        private Animator animator;
        private static int _stateNameHash = Animator.StringToHash("ChestOpenClose");
        private static int _chestParameter = Animator.StringToHash("Open");

        private AnimatorStateInfo chestState;
        private bool chestOn;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            TEST();
        }

        private void TEST()
        {
            if (!Input.GetKeyDown(KeyCode.O)) return;
            chestState = animator.GetCurrentAnimatorStateInfo(0);
            chestOn = AnimationTools.PlayAnimation(
                animator, _stateNameHash, _chestParameter, chestOn, chestState.normalizedTime);
        }
    }
}