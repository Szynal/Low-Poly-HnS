using LowPolyHnS.Core;
using UnityEngine.Timeline;

namespace LowPolyHnS.Playables
{
    [TrackColor(0.76f, 0.76f, 0.76f)]
    [TrackClipType(typeof(TriggerAsset))]
    [TrackBindingType(typeof(Trigger))]
    public class TriggerTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Trigger";
            clip.duration = 1.0f;
        }
    }
}