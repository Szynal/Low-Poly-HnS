using LowPolyHnS.Core;
using UnityEngine.Timeline;

namespace LowPolyHnS.Playables
{
    [TrackColor(0.8f, 0.76f, 0.64f)]
    [TrackClipType(typeof(ActionsAsset))]
    [TrackBindingType(typeof(Actions))]
    public class ActionsTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Actions";
            clip.duration = 1.0f;
        }
    }
}