using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionAudioPause : IAction
    {
        public BoolProperty pause = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioListener.pause = pause.GetValue(target);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Pause Audio";
        private const string NODE_TITLE = "Play/Pause audio";

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

#endif
    }
}