using UnityEngine;
using UnityEngine.Playables;

namespace LowPolyHnS.Playables
{
    public abstract class IGenericBehavior<T> : PlayableBehaviour
    {
        protected T interactable;
        protected bool execute;

        public GameObject invoker;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            execute = true;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;
            interactable = (T) playerData;

            if (interactable != null && execute)
            {
                Execute();
                execute = false;
            }
        }

        protected abstract void Execute();
    }
}