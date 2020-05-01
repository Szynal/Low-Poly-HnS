using System;
using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class AudioBuffer
    {
        private const float ZERO = 0.001f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private AudioSource audio;

        private int indexVolume;
        private float clipVolume;
        private float smoothTime;

        private float opacityValue;
        private float opacityTarget;
        private float opacityVelocity;

        // INITIALIZE: ----------------------------------------------------------------------------

        public AudioBuffer(AudioSource audio, int indexVolume)
        {
            this.audio = audio;
            clipVolume = 1f;
            this.indexVolume = indexVolume;
            smoothTime = 1f;

            opacityValue = 1f;
            opacityTarget = 1f;
            opacityVelocity = 0f;
        }

        // UPDATE: --------------------------------------------------------------------------------

        public void Update()
        {
            if (!audio.clip || !audio.isPlaying) return;

            opacityValue = Mathf.SmoothDamp(
                opacityValue,
                opacityTarget,
                ref opacityVelocity,
                smoothTime
            );

            audio.volume = opacityValue * clipVolume *
                           AudioManager.Instance.GetGlobalVolume(indexVolume) *
                           AudioManager.Instance.GetGlobalVolume(AudioManager.INDEX_VOLUME_MASTR);

            if (Mathf.Approximately(opacityTarget + opacityValue, 0f))
            {
                audio.Stop();
                audio.clip = null;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Play(AudioClip audioClip, float fadeDuration = 0.0f,
            float clipVolume = 1.0f, AudioMixerGroup mixer = null)
        {
            smoothTime = fadeDuration;
            this.clipVolume = clipVolume;

            audio.clip = audioClip;
            audio.outputAudioMixerGroup = mixer;
            audio.Play();

            opacityValue = fadeDuration > ZERO ? 0f : 1f;
            opacityTarget = 1f;
            opacityVelocity = 0f;
        }

        public void Stop(float fadeDuration = 0.0f)
        {
            smoothTime = fadeDuration;

            opacityTarget = 0f;
            opacityVelocity = 0f;

            if (fadeDuration <= ZERO)
            {
                audio.Stop();
                audio.clip = null;
            }
        }

        public AudioClip GetAudioClip()
        {
            return audio.clip;
        }

        public void SetPosition(Vector3 position)
        {
            audio.transform.position = position;
        }

        public void SetPitch(float pitch)
        {
            audio.pitch = pitch;
        }

        public void SetSpatialBlend(float spatialBlend)
        {
            audio.spatialize = !Mathf.Approximately(spatialBlend, 0);
            audio.spatialBlend = spatialBlend;
        }
    }
}