namespace LowPolyHnS.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using LowPolyHnS.Core;

    public class ActionsBehavior : IGenericBehavior<Actions>
    {
        protected override void Execute()
        {
            this.interactable.Execute(this.invoker);
        }
    }
}