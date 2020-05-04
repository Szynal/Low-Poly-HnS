using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class ActionCharacterTeleport : IAction
    {
        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space] public TargetPosition position = new TargetPosition(TargetPosition.Target.Position);
        public bool rotate = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character targetCharacter = character.GetCharacter(target);
            if (targetCharacter == null) return true;

            Vector3 targetPosition = position.GetPosition(target);

            switch (rotate)
            {
                case true:
                    targetCharacter.characterLocomotion.Teleport(
                        targetPosition,
                        position.GetRotation(target)
                    );
                    break;

                case false:
                    targetCharacter.characterLocomotion.Teleport(targetPosition);
                    break;
            }

            return true;
        }

#if UNITY_EDITOR

        public static new string NAME = "Character/Teleport";
        private const string NODE_TITLE = "Teleport {0} to {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, character, position);
        }

#endif
    }
}