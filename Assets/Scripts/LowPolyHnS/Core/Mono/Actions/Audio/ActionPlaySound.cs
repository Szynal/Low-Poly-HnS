using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionPlaySound : IAction
    {
        public enum AudioMixerType
        {
            None,
            Custom,
            DefaultSoundMixer
        }

        public AudioClip audioClip;
        public AudioMixerType audioMixer = AudioMixerType.DefaultSoundMixer;
        [Indent] public AudioMixerGroup mixerGroup;

        [Space] public float fadeIn;
        [Range(0f, 1f)] public float volume = 1.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioMixerGroup mixer = null;
            switch (audioMixer)
            {
                case AudioMixerType.DefaultSoundMixer:
                    mixer = DatabaseGeneral.Load().soundAudioMixer;
                    break;

                case AudioMixerType.Custom:
                    mixer = mixerGroup;
                    break;
            }

            AudioManager.Instance.PlaySound2D(audioClip, fadeIn, volume, mixer);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Play Sound";
        private const string NODE_TITLE = "Play Sound {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                audioClip == null ? "(none)" : audioClip.name
            );
        }

        private SerializedProperty spAudioClip;
        private SerializedProperty spAudioMixer;
        private SerializedProperty spMixerGroup;

        private SerializedProperty spFadeIn;
        private SerializedProperty spVolume;

        protected override void OnEnableEditorChild()
        {
            spAudioClip = serializedObject.FindProperty("audioClip");
            spAudioMixer = serializedObject.FindProperty("audioMixer");
            spMixerGroup = serializedObject.FindProperty("mixerGroup");
            spFadeIn = serializedObject.FindProperty("fadeIn");
            spVolume = serializedObject.FindProperty("volume");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spAudioClip);
            EditorGUILayout.PropertyField(spAudioMixer);
            if (spAudioMixer.enumValueIndex == (int) AudioMixerType.Custom)
            {
                EditorGUILayout.PropertyField(spMixerGroup);
            }

            EditorGUILayout.PropertyField(spFadeIn);
            EditorGUILayout.PropertyField(spVolume);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}