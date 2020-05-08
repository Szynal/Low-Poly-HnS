using LowPolyHnS.Characters;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnLand : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "Character/On Land";
#endif

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        [Space] [VariableFilter(Variable.DataType.Number)]
        public VariableProperty storeVerticalSpeed = new VariableProperty();

        private void Start()
        {
            Character target = character.GetCharacter(gameObject);
            if (target != null)
            {
                target.onLand.AddListener(OnLand);
            }
        }

        private void OnLand(float verticalSpeed)
        {
            Character instance = character.GetCharacter(gameObject);

            storeVerticalSpeed.Set(verticalSpeed, instance.gameObject);
            base.ExecuteTrigger(instance.gameObject);
        }
    }
}