using UnityEngine;

namespace LowPolyHnS
{
    public static class AnimationTools
    {
        public static bool PlayAnimation(Animator animator, int stateNameHash, int parameterNameHash, bool typeOn,
            float normalizedTime)
        {
            float playTime = normalizedTime % 1;

            if (typeOn && normalizedTime > 1)
            {
                playTime = 1.0f;
            }
            else if (!typeOn && (normalizedTime > 1 || normalizedTime < 0))
            {
                playTime = 0.0f;
            }

            if (animator.GetFloat(parameterNameHash) >= 1)
            {
                typeOn = false;
                animator.SetFloat(parameterNameHash, -1);
                animator.Play(stateNameHash, -1, playTime);
            }
            else
            {
                typeOn = true;
                animator.SetFloat(parameterNameHash, 1);
                animator.Play(stateNameHash, -1, playTime);
            }

            return typeOn;
        }
    }
}