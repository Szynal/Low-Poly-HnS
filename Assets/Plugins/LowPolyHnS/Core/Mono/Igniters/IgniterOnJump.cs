﻿using LowPolyHnS.Characters;
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
            Character target = this.character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onJump.AddListener(this.OnJump);
            }
        }

        private void OnJump(int chain)
        {
            if (this.jumpChain < 0 || this.jumpChain == chain)
            {
                base.ExecuteTrigger(this.character.GetCharacter(gameObject).gameObject);
            }
        }
    }
}