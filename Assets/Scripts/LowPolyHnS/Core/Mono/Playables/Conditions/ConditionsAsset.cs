using System;
using UnityEngine;
using UnityEngine.Playables;

namespace LowPolyHnS.Playables
{
    [Serializable]
    public class ConditionsAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ConditionsBehavior> playable = ScriptPlayable<ConditionsBehavior>.Create(graph);
            ConditionsBehavior behavior = playable.GetBehaviour();
            behavior.invoker = owner;

            return playable;
        }
    }
}