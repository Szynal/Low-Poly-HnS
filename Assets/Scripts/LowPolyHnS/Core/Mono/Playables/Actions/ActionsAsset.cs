using System;
using UnityEngine;
using UnityEngine.Playables;

namespace LowPolyHnS.Playables
{
    [Serializable]
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