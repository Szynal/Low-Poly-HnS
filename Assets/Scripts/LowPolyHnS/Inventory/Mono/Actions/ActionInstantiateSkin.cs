using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ActionInstantiateSkin : IAction
    {
        public enum Operation
        {
            Add,
            Remove
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Invoker);
        [Space] public Operation action = Operation.Add;
        public GameObjectProperty prefab = new GameObjectProperty();

        [Space, Tooltip("(Optional) store the garment instance in a variable")]
        public VariableProperty storeReference = new VariableProperty();

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject instance = null;
            switch (action)
            {
                case Operation.Add:
                    instance = SkinmeshManager.Wear(
                        prefab.GetValue(target),
                        character.GetCharacter(target)
                    );
                    break;

                case Operation.Remove:
                    instance = SkinmeshManager.TakeOff(
                        prefab.GetValue(target),
                        character.GetCharacter(target)
                    );
                    break;
            }

            storeReference.Set(instance, target);
            return true;
        }

#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Actions/";

        public static new string NAME = "Inventory/Instantiate Skinned Mesh";
        private const string NODE_TITLE = "Add skin-mesh {0} on {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, prefab, character);
        }

#endif
    }
}