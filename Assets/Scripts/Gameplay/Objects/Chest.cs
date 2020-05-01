using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [RequireComponent(typeof(Animator))]
    public class Chest : MonoBehaviour
    {
        private Animator animator;
        private static int STATE_NAME_HASH = Animator.StringToHash("ChestOpenClose");
        private static int CHEST_PARAMETER = Animator.StringToHash("Open");

        private AnimatorStateInfo chestState;
        private bool chestOn;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimateChest()
        {
            chestState = animator.GetCurrentAnimatorStateInfo(0);
            chestOn = AnimationTools.PlayAnimation(animator, STATE_NAME_HASH, CHEST_PARAMETER, chestOn,
                chestState.normalizedTime);
        }
    }
}