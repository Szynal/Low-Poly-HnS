using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionStopMusic : IAction
    {
        public enum MusicType
        {
            AllMusic,
            AudioClip
        }

        public MusicType type = MusicType.AllMusic;
        [Indent] public AudioClip audioClip;

        [Range(0f, 10f)] public float fadeOut;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            switch (type)
            {
                case MusicType.AllMusic:
                    AudioManager.Instance.StopAllMusic(fadeOut);
                    break;

                case MusicType.AudioClip:
                    AudioManager.Instance.StopMusic(audioClip, fadeOut);
                    break;
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Audio/Stop Music";
        private const string NODE_TITLE = "Stop Music {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                fadeOut > 0f ? "(" + fadeOut + "s)" : ""
            );
        }

        private SerializedProperty spType;
        private SerializedProperty spAudioClip;
        private SerializedProperty spFadeOut;

        protected override void OnEnableEditorChild()
        {
            spType = serializedObject.FindProperty("type");
            spAudioClip = serializedObject.FindProperty("audioClip");
            spFadeOut = serializedObject.FindProperty("fadeOut");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spType);
            if (spType.enumValueIndex == (int) MusicType.AudioClip)
            {
                EditorGUILayout.PropertyField(spAudioClip);
            }

            EditorGUILayout.PropertyField(spFadeOut);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}