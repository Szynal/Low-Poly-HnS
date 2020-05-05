using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionAudioMixerParameter : IAction
    {
        public AudioMixer audioMixer;

        [Space] public string parameter = "MyParameter";
        public NumberProperty value = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (audioMixer)
            {
                audioMixer.SetFloat(parameter, value.GetValue(target));
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Audio Mixer Parameter";
        private const string NODE_TITLE = "Change {0} mixer {1} parameter";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                audioMixer != null ? audioMixer.name : "(none)",
                parameter
            );
        }

#endif
    }
}