using LowPolyHnS.Core;
using UnityEngine.Timeline;

namespace LowPolyHnS.Playables
{
    [TrackColor(0.59f, 0.79f, 0.75f)]
    [TrackClipType(typeof(ConditionsAsset))]
    [TrackBindingType(typeof(Conditions))]
    public class ConditionsTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Conditions";
            clip.duration = 1.0f;
        }
    }
}