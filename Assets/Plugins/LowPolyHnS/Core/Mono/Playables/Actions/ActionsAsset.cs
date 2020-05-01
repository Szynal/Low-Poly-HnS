namespace LowPolyHnS.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using LowPolyHnS.Core;

    [System.Serializable]
    public class ActionsAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ActionsBehavior> playable = ScriptPlayable<ActionsBehavior>.Create(graph);
            ActionsBehavior behavior = playable.GetBehaviour();
            behavior.invoker = owner;

            return playable;
        }
    }
}