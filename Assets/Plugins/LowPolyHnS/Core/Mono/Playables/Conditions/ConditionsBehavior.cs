namespace LowPolyHnS.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using LowPolyHnS.Core;

    public class ConditionsBehavior : IGenericBehavior<Conditions>
    {
        protected override void Execute()
        {
            this.interactable.Interact(this.invoker);
        }
    }
}