using System;
using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Managers/AudioManager", 100)]
    public class AudioManager : Singleton<AudioManager>, IGameSave
    {
        [Serializable]
        public class Volume
        {
            public float mastr;
            public float music;
            public float sound;
            public float voice;

            public Volume(float mastr, float music, float sound, float voice)
            {
                this.mastr = mastr;
                this.music = music;
                this.sound = sound;
                this.voice = voice;
            }
        }

        private const int MAX_MUSIC_SOURCES = 10;
        private const int MAX_SOUND_SOURCES = 50;
        private const int MAX_VOICE_SOURCES = 10;

        private static float[] GLOBAL_VOLUMES =
        {
            1.0f, // mastr
            1.0f, // music
            1.0f, // sound
            1.0f // voice
        };

        public const int INDEX_VOLUME_MASTR = 0;
        public const int INDEX_VOLUME_MUSIC = 1;
        public const int INDEX_VOLUME_SOUND = 2;
        public const int INDEX_VOLUME_VOICE = 3;

        // PROPERTIES: ----------------------------------------------------------------------------

        private int musicIndex;
        private int sound2DIndex;
        private int sound3DIndex;
        private int voiceIndex;

        private AudioBuffer[] musicSources;
        private AudioBuffer[] sound2DSources;
        private AudioBuffer[] sound3DSources;
        private AudioBuffer[] voiceSources;

        // INITIALIZE: ----------------------------------------------------------------------------

        protected override void OnCreate()
        {
            musicSources = new AudioBuffer[MAX_MUSIC_SOURCES];
            sound2DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            sound3DSources = new AudioBuffer[MAX_SOUND_SOURCES];
            voiceSources = new AudioBuffer[MAX_VOICE_SOURCES];

            for (int i = 0; i < musicSources.Length; ++i) musicSources[i] = CreateMusicSource(i);
            for (int i = 0; i < sound2DSources.Length; ++i) sound2DSources[i] = CreateSoundSource(i, "2D");
            for (int i = 0; i < sound3DSources.Length; ++i) sound3DSources[i] = CreateSoundSource(i, "3D");
            for (int i = 0; i < voiceSources.Length; ++i) voiceSources[i] = CreateVoiceSource(i);

            SaveLoadManager.Instance.Initialize(this);
        }

        private AudioBuffer CreateMusicSource(int index)
        {
            AudioSource clip = CreateAudioAsset("music", index, true);
            return new AudioBuffer(clip, INDEX_VOLUME_MUSIC);
        }

        private AudioBuffer CreateSoundSource(int index, string suffix)
        {
            AudioSource clip = CreateAudioAsset("sound" + suffix, index, false);
            return new AudioBuffer(clip, INDEX_VOLUME_SOUND);
        }

        private AudioBuffer CreateVoiceSource(int index)
        {
            AudioSource clip = CreateAudioAsset("voice", index, false);
            return new AudioBuffer(clip, INDEX_VOLUME_VOICE);
        }

        private AudioSource CreateAudioAsset(string audioName, int index, bool loop)
        {
            GameObject asset = new GameObject(audioName + "_" + index);
            asset.transform.parent = transform;

            AudioSource clip = asset.AddComponent<AudioSource>();
            clip.playOnAwake = false;
            clip.loop = loop;

            return clip;
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            for (int i = 0; i < musicSources.Length; ++i)
            {
                musicSources[i].Update();
            }

            for (int i = 0; i < sound2DSources.Length; ++i)
            {
                sound2DSources[i].Update();
            }

            for (int i = 0; i < sound3DSources.Length; ++i)
            {
                sound3DSources[i].Update();
            }

            for (int i = 0; i < voiceSources.Length; ++i)
            {
                voiceSources[i].Update();
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void PlayMusic(AudioClip audioClip, float fadeIn = 0f, float volume = 1f, AudioMixerGroup mixer = null)
        {
            musicIndex = ++musicIndex < musicSources.Length ? musicIndex : 0;
            musicSources[musicIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopMusic(AudioClip audioClip, float fadeOut = 0.0f)
        {
            for (int i = 0; i < musicSources.Length; ++i)
            {
                if (musicSources[i].GetAudioClip() == audioClip)
                {
                    musicSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopAllMusic(float fadeOut = 0.0f)
        {
            for (int i = 0; i < musicSources.Length; ++i)
            {
                musicSources[i].Stop(fadeOut);
            }
        }

        public void PlaySound2D(AudioClip audioClip, float fadeIn = 0f,
            float volume = 1f, AudioMixerGroup mixer = null)
        {
            sound2DIndex = ++sound2DIndex < sound2DSources.Length ? sound2DIndex : 0;
            sound2DSources[sound2DIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopSound2D(AudioClip audioClip, float fadeOut = 0f)
        {
            for (int i = 0; i < sound2DSources.Length; ++i)
            {
                AudioClip clip = sound2DSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    sound2DSources[i].Stop(fadeOut);
                }
            }
        }

        public void PlaySound3D(AudioClip audioClip, float fadeIn, Vector3 position,
            float spatialBlend, float pitch, float volume = 1f, AudioMixerGroup mixer = null)
        {
            sound3DIndex = ++sound3DIndex < sound3DSources.Length ? sound3DIndex : 0;
            sound3DSources[sound3DIndex].SetSpatialBlend(spatialBlend);
            sound3DSources[sound3DIndex].SetPitch(pitch);
            sound3DSources[sound3DIndex].SetPosition(position);
            sound3DSources[sound3DIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopSound3D(AudioClip audioClip, float fadeOut = 0f)
        {
            for (int i = 0; i < sound3DSources.Length; ++i)
            {
                AudioClip clip = sound3DSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    sound3DSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopSound(AudioClip audioClip, float fadeOut = 0f)
        {
            StopSound2D(audioClip, fadeOut);
            StopSound3D(audioClip, fadeOut);
        }

        public void StopAllSounds(float fadeOut = 0f)
        {
            for (int i = 0; i < sound2DSources.Length; ++i)
            {
                sound2DSources[i].Stop(fadeOut);
            }

            for (int i = 0; i < sound3DSources.Length; ++i)
            {
                sound3DSources[i].Stop(fadeOut);
            }
        }

        public void PlayVoice(AudioClip audioClip, float fadeIn = 0f,
            float volume = 1f, AudioMixerGroup mixer = null)
        {
            voiceIndex = ++voiceIndex < voiceSources.Length ? voiceIndex : 0;
            voiceSources[voiceIndex].Play(audioClip, fadeIn, volume, mixer);
        }

        public void StopVoice(AudioClip audioClip, float fadeOut = 0f)
        {
            for (int i = 0; i < voiceSources.Length; ++i)
            {
                AudioClip clip = voiceSources[i].GetAudioClip();
                if (clip != null && clip.name == audioClip.name)
                {
                    voiceSources[i].Stop(fadeOut);
                }
            }
        }

        public void StopAllVoices(float fadeOut = 0f)
        {
            for (int i = 0; i < voiceSources.Length; ++i)
            {
                voiceSources[i].Stop(fadeOut);
            }
        }

        public void StopAllAudios(float fadeOut = 0f)
        {
            StopAllMusic(fadeOut);
            StopAllSounds(fadeOut);
            StopAllVoices(fadeOut);
        }

        // VOLUME METHODS: ------------------------------------------------------------------------

        public void SetGlobalVolume(int index, float volume)
        {
            GLOBAL_VOLUMES[index] = volume;
        }

        public void SetGlobalMastrVolume(float volume)
        {
            SetGlobalVolume(INDEX_VOLUME_MASTR, volume);
        }

        public void SetGlobalMusicVolume(float volume)
        {
            SetGlobalVolume(INDEX_VOLUME_MUSIC, volume);
        }

        public void SetGlobalSoundVolume(float volume)
        {
            SetGlobalVolume(INDEX_VOLUME_SOUND, volume);
        }

        public void SetGlobalVoiceVolume(float volume)
        {
            SetGlobalVolume(INDEX_VOLUME_VOICE, volume);
        }

        public float GetGlobalVolume(int index)
        {
            return GLOBAL_VOLUMES[index];
        }

        public float GetGlobalMastrVolume()
        {
            return GetGlobalVolume(INDEX_VOLUME_MASTR);
        }

        public float GetGlobalMusicVolume()
        {
            return GetGlobalVolume(INDEX_VOLUME_MUSIC);
        }

        public float GetGlobalSoundVolume()
        {
            return GetGlobalVolume(INDEX_VOLUME_SOUND);
        }

        public float GetGlobalVoiceVolume()
        {
            return GetGlobalVolume(INDEX_VOLUME_VOICE);
        }

        // INTERFACE ISAVELOAD: -------------------------------------------------------------------

        public string GetUniqueName()
        {
            return "volume";
        }

        public Type GetSaveDataType()
        {
            return typeof(Volume);
        }

        public object GetSaveData()
        {
            return new Volume(
                GetGlobalMastrVolume(),
                GetGlobalMusicVolume(),
                GetGlobalSoundVolume(),
                GetGlobalVoiceVolume()
            );
        }

        public void ResetData()
        {
            Instance.SetGlobalMastrVolume(1.0f);
            Instance.SetGlobalMusicVolume(1.0f);
            Instance.SetGlobalSoundVolume(1.0f);
            Instance.SetGlobalVoiceVolume(1.0f);
        }

        public void OnLoad(object generic)
        {
            Volume volume = generic as Volume;
            if (volume == null) return;

            Instance.SetGlobalMastrVolume(volume.mastr);
            Instance.SetGlobalMusicVolume(volume.music);
            Instance.SetGlobalSoundVolume(volume.sound);
            Instance.SetGlobalVoiceVolume(volume.voice);
        }
    }
}