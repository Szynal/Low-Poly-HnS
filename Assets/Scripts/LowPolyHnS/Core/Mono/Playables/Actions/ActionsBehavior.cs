using LowPolyHnS.Core;

namespace LowPolyHnS.Playables
{
    public class ActionsBehavior : IGenericBehavior<Actions>
    {
        protected override void Execute()
        {
            interactable.Execute(invoker);
        }
    }
}