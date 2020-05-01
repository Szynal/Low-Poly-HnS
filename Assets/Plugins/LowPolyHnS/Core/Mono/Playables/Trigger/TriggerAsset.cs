namespace LowPolyHnS.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using LowPolyHnS.Core;

    [System.Serializable]
    public class TriggerAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TriggerBehavior> playable = ScriptPlayable<TriggerBehavior>.Create(graph);
            TriggerBehavior behavior = playable.GetBehaviour();
            behavior.invoker = owner;

            return playable;
        }
    }
}