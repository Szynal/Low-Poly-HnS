using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class ActionCharacterIK : IAction
    {
        public enum Section
        {
            Head,
            Hands,
            Feet
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space] public Section part = Section.Head;
        public bool enable = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character instance = character.GetCharacter(target);
            if (instance == null) return true;

            CharacterAnimator animator = instance.GetCharacterAnimator();
            if (animator == null) return true;

            switch (part)
            {
                case Section.Head:
                    animator.useSmartHeadIK = enable;
                    break;
                case Section.Hands:
                    animator.useHandIK = enable;
                    break;
                case Section.Feet:
                    animator.useFootIK = enable;
                    break;
            }

            return true;
        }

#if UNITY_EDITOR

        public static new string NAME = "Character/Inverse Kinematics";
        private const string NODE_TITLE = "Set {0} {1} IK as {2}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, character, part, enable.ToString());
        }

#endif
    }
}