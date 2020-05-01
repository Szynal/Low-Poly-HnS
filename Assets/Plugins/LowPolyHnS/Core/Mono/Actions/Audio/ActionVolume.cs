using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionVolume : IAction
    {
        public enum VolumeType
        {
            Master = AudioManager.INDEX_VOLUME_MASTR,
            Music = AudioManager.INDEX_VOLUME_MUSIC,
            Sound = AudioManager.INDEX_VOLUME_SOUND,
            Voice = AudioManager.INDEX_VOLUME_VOICE
        }

        public VolumeType type = VolumeType.Music;
        public NumberProperty volume = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float value = volume.GetValue(target);
            AudioManager.Instance.SetGlobalVolume((int) type, value);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Change Volume";
        private const string NODE_TITLE = "Change {0} volume to {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, type, volume);
        }

#endif
    }
}