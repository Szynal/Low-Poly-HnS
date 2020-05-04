using System.Collections;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace LowPolyHnS.Characters
{
    public abstract class PlayableBase
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected float fadeIn;
        protected float fadeOut;

        protected float speed;
        protected float weight;

        private AvatarMask avatarMask;
        private AnimationLayerMixerPlayable mixer;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public Playable Mixer => mixer;

        public Playable Input0 => mixer.GetInput(0);
        public Playable Input1 => mixer.GetInput(1);
        public Playable Output => mixer.GetOutput(0);

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected PlayableBase(AvatarMask avatarMask, float fadeIn, float fadeOut, float speed, float weight)
        {
            this.avatarMask = avatarMask;

            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;

            this.speed = speed;
            this.weight = weight;
        }

        public void Destroy()
        {
            Playable output = Output;
            Playable input0 = Input0;

            output.DisconnectInput(0);
            mixer.DisconnectInput(0);

            output.ConnectInput(0, input0, 0);

            switch (output.GetInputCount())
            {
                case 1:
                    output.SetInputWeight(0, 1f);
                    break;

                case 2:
                    float outputWeight = mixer.GetInputWeight(0);
                    output.SetInputWeight(0, outputWeight);
                    break;
            }

            IEnumerator destroy = DestroyNextFrame();
            CoroutinesManager.Instance.StartCoroutine(destroy);
        }

        private IEnumerator DestroyNextFrame()
        {
            yield return null;

            if (Input1.IsValid() && Input1.CanDestroy()) Input1.Destroy();
            if (Mixer.IsValid() && Mixer.CanDestroy()) Mixer.Destroy();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Stop(float fadeOut)
        {
            this.fadeOut = fadeOut;
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract bool Update();

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void Setup<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            input0.GetOutput(0).DisconnectInput(0);
            output.DisconnectInput(0);

            SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void Setup<TInput1>(
            ref PlayableGraph graph, PlayableBase previous, ref TInput1 input1)
            where TInput1 : struct, IPlayable
        {
            Playable input0 = previous.Mixer;
            Playable output = previous.Output;
            previous.Output.DisconnectInput(0);

            SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void Setup<TInput1>(
            ref PlayableGraph graph, ref TInput1 input1, PlayableBase next)
            where TInput1 : struct, IPlayable
        {
            Playable input0 = next.Input0;
            Playable output = next.Mixer;
            output.DisconnectInput(0);

            SetupMixer(ref graph, ref input0, ref input1, ref output);
        }

        protected void UpdateMixerWeights(float weight)
        {
            float weight0 = 1f;
            float weight1 = weight;

            mixer.SetInputWeight(0, weight0);
            mixer.SetInputWeight(1, weight1);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SetupMixer<TInput0, TInput1, TOutput>(
            ref PlayableGraph graph, ref TInput0 input0, ref TInput1 input1, ref TOutput output)
            where TInput0 : struct, IPlayable
            where TInput1 : struct, IPlayable
            where TOutput : struct, IPlayable
        {
            input1.SetSpeed(speed);

            mixer = AnimationLayerMixerPlayable.Create(graph, 2);
            mixer.ConnectInput(0, input0, 0, 0f);
            mixer.ConnectInput(1, input1, 0, 1f);

            if (avatarMask != null)
            {
                mixer.SetLayerMaskFromAvatarMask(1, avatarMask);
            }

            output.ConnectInput(0, mixer, 0, 1f);
            UpdateMixerWeights(fadeIn > CharacterAnimation.EPSILON
                ? 0f
                : weight
            );
        }
    }
}