using UnityEngine;

namespace LowPolyHnS.Characters
{
    public class CharacterState : ScriptableObject
    {
        public enum StateType
        {
            Simple,
            Locomotion,
            Other
        }

        public StateType type = StateType.Simple;

        public AnimatorOverrideController rtcSimple;
        public AnimatorOverrideController rtcLocomotion;
        public RuntimeAnimatorController rtcOther;

        public AnimationClip enterClip;
        public AvatarMask enterAvatarMask;

        public AnimationClip exitClip;
        public AvatarMask exitAvatarMask;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public RuntimeAnimatorController GetRuntimeAnimatorController()
        {
            switch (type)
            {
                case StateType.Simple: return rtcSimple;
                case StateType.Locomotion: return rtcLocomotion;
                case StateType.Other: return rtcOther;
            }

            return null;
        }

        public void StartState(CharacterAnimation character)
        {
            if (character == null) return;
            if (enterClip == null) return;

            character.CrossFadeGesture(enterClip, enterAvatarMask, 0.15f, 0.15f, 1.0f);
        }

        public void ExitState(CharacterAnimation character)
        {
            if (character == null) return;
            if (exitClip == null) return;

            character.CrossFadeGesture(exitClip, exitAvatarMask, 0.15f, 0.15f, 1.0f);
        }
    }
}