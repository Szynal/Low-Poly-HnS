using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionStopSound : IAction
    {
        public AudioClip audioClip;

        [Range(0f, 5f)] public float fadeOut;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioManager.Instance.StopSound(audioClip, fadeOut);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Stop Sound";
        private const string NODE_TITLE = "Stop Sound {0} {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                audioClip == null ? "unknown" : audioClip.name,
                fadeOut > 0f ? "(" + fadeOut + "s)" : ""
            );
        }

#endif
    }
}