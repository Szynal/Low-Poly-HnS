using UnityEngine;
using UnityEngine.Playables;

namespace LowPolyHnS.Characters
{
    public abstract class PlayableState : PlayableBase
    {
        public int Layer { get; protected set; }

        public AnimationClip AnimationClip { get; protected set; }
        public CharacterState CharacterState { get; protected set; }

        protected bool isDisposing;
        private float currentWeight;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected PlayableState(AvatarMask avatarMask,
            int layer, float time, float speed, float weight)
            : base(avatarMask, time, 0f, speed, weight)
        {
            Layer = layer;
        }

        // UPDATE: --------------------------------------------------------------------------------

        public override bool Update()
        {
            if (Input1.IsDone())
            {
                Stop(0f);
                return true;
            }

            float increment = currentWeight < weight
                ? fadeIn
                : -fadeOut;

            if (Mathf.Abs(increment) < float.Epsilon) currentWeight = weight;
            else currentWeight += Time.deltaTime / increment;

            UpdateMixerWeights(Mathf.Clamp01(currentWeight));
            return isDisposing && currentWeight < float.Epsilon;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override void Stop(float fadeOut)
        {
            base.Stop(fadeOut);

            isDisposing = true;
            weight = 0f;
        }

        public void StretchDuration(float freezeTime)
        {
            double duration = Input1.GetTime() + freezeTime;
            Input1.SetDuration(duration);
            Input1.SetSpeed(1f);
        }

        public void SetWeight(float weight)
        {
            this.weight = weight;
        }

        public virtual void OnExitState()
        {
        }
    }
}