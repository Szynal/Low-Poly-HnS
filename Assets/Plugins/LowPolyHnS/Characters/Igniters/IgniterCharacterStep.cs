using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class IgniterCharacterStep : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Character/On Step";
#endif

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public CharacterLocomotion.STEP step = CharacterLocomotion.STEP.Any;

        private void Start()
        {
            Character target = character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onStep.AddListener(OnStep);
            }
        }

        private void OnDestroy()
        {
            if (isExitingApplication) return;
            Character target = character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onStep.RemoveListener(OnStep);
            }
        }

        private void OnStep(CharacterLocomotion.STEP stepType)
        {
            if (step != CharacterLocomotion.STEP.Any && stepType != CharacterLocomotion.STEP.Any)
            {
                if (step != stepType) return;
            }

            Character target = character.GetCharacter(gameObject);
            ExecuteTrigger(target.gameObject);
        }
    }
}