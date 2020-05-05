using LowPolyHnS.Core;

namespace LowPolyHnS.Playables
{
    public class ConditionsBehavior : IGenericBehavior<Conditions>
    {
        protected override void Execute()
        {
            interactable.Interact(invoker);
        }
    }
}