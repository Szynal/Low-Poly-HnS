using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionPlayMusic : IAction
    {
        public enum AudioMixerType
        {
            None,
            Custom,
            DefaultMusicMixer
        }

        public AudioClip audioClip;
        public AudioMixerType audioMixer = AudioMixerType.DefaultMusicMixer;
        [Indent] public AudioMixerGroup mixerGroup;

        [Space] public float fadeIn = 1f;

        [Range(0f, 1f)] public float volume = 1f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioMixerGroup mixer = null;
            switch (audioMixer)
            {
                case AudioMixerType.DefaultMusicMixer:
                    mixer = DatabaseGeneral.Load().musicAudioMixer;
                    break;

                case AudioMixerType.Custom:
                    mixer = mixerGroup;
                    break;
            }

            AudioManager.Instance.PlayMusic(audioClip, fadeIn, volume, mixer);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Play Music";
        private const string NODE_TITLE = "Play Music {0} {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                audioClip == null ? "none" : audioClip.name,
                fadeIn > 0f ? "(" + fadeIn + "s)" : ""
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