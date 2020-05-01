using LowPolyHnS.Core;

namespace LowPolyHnS.Playables
{
    public class TriggerBehavior : IGenericBehavior<Trigger>
    {
        protected override void Execute()
        {
            interactable.Execute(invoker);
        }
    }
}