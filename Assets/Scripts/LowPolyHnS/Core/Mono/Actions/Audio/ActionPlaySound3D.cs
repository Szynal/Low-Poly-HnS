using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionPlaySound3D : IAction
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

        [Range(0f, 10f)] public float fadeIn;

        [Range(0.0f, 1.0f)] public float volume = 1f;

        [Range(0.0f, 1.0f)] public float spatialBlend = 0.85f;
        public NumberProperty pitch = new NumberProperty(1.0f);
        public TargetPosition position = new TargetPosition(TargetPosition.Target.Player);

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

            AudioManager.Instance.PlaySound3D(
                audioClip,
                fadeIn,
                position.GetPosition(target),
                spatialBlend,
                pitch.GetValue(target),
                volume,
                mixer
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Play Sound 3D";
        private const string NODE_TITLE = "Play 3D Sound {0} {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                audioClip == null ? "(none)" : audioClip.name,
                fadeIn > 0f ? "(" + fadeIn + "s)" : ""
            );
        }

        private SerializedProperty spAudioClip;
        private SerializedProperty spAudioMixer;
        private SerializedProperty spMixerGroup;

        private SerializedProperty spFadeIn;
        private SerializedProperty spVolume;
        private SerializedProperty spSpatialBlend;
        private SerializedProperty spPitch;
        private SerializedProperty spPosition;

        protected override void OnEnableEditorChild()
        {
            spAudioClip = serializedObject.FindProperty("audioClip");
            spAudioMixer = serializedObject.FindProperty("audioMixer");
            spMixerGroup = serializedObject.FindProperty("mixerGroup");
            spFadeIn = serializedObject.FindProperty("fadeIn");
            spVolume = serializedObject.FindProperty("volume");
            spSpatialBlend = serializedObject.FindProperty("spatialBlend");
            spPitch = serializedObject.FindProperty("pitch");
            spPosition = serializedObject.FindProperty("position");
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

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spFadeIn);
            EditorGUILayout.PropertyField(spVolume);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spSpatialBlend);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spPitch);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spPosition);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}