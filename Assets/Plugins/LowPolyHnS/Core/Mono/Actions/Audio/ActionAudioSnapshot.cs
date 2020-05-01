using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionAudioSnapshot : IAction
    {
        public AudioMixerSnapshot snapshot;
        public float duration = 0.5f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (snapshot != null)
            {
                snapshot.TransitionTo(duration);
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Audio Snapshot";
        private const string NODE_TITLE = "Transition to {0} in {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                snapshot != null ? snapshot.name : "(none)",
                duration
            );
        }

#endif
    }
}