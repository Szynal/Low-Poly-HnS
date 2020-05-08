using LowPolyHnS.Characters;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnJump : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Character/On Jump";
#endif

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);
        public int jumpChain = 0;

        private void Start()
        {
            Character target = character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onJump.AddListener(OnJump);
            }
        }

        private void OnJump(int chain)
        {
            if (jumpChain < 0 || jumpChain == chain)
            {
                base.ExecuteTrigger(character.GetCharacter(gameObject).gameObject);
            }
        }
    }
}