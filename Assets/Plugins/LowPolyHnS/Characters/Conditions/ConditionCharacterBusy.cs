using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class ConditionCharacterBusy : ICondition
    {
        public enum Busy
        {
            IsBusy,
            IsNotBusy
        }

        public TargetCharacter character = new TargetCharacter();
        public Busy state = Busy.IsBusy;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
        {
            Character charTarget = character.GetCharacter(target);
            if (charTarget == null) return false;

            bool isBusy = charTarget.characterLocomotion.isBusy;
            switch (state)
            {
                case Busy.IsBusy: return isBusy;
                case Busy.IsNotBusy: return !isBusy;
            }

            return false;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Characters/Character Busy";
        private const string NODE_TITLE = "Character {0} {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                character,
                ObjectNames.NicifyVariableName(state.ToString())
            );
        }

#endif
    }
}