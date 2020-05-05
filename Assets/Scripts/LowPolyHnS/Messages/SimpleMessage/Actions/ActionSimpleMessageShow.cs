using System.Collections;
using LowPolyHnS.Core;
using LowPolyHnS.Localization;
using UnityEngine;
using UnityEngine.Audio;

namespace LowPolyHnS.Messages
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionSimpleMessageShow : IAction
    {
        public AudioClip audioClip;

        [LocStringNoPostProcess] public LocString message = new LocString();
        public Color color = Color.white;
        public float time = 2.0f;

        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (audioClip != null)
            {
                AudioMixerGroup voiceMixer = DatabaseGeneral.Load().voiceAudioMixer;
                AudioManager.Instance.PlayVoice(audioClip, 0f, 1f, voiceMixer);
            }

            SimpleMessageManager.Instance.ShowText(message.GetText(), color);

            float waitTime = Time.time + time;
            WaitUntil waitUntil = new WaitUntil(() => Time.time > waitTime || forceStop);
            yield return waitUntil;

            if (audioClip != null) AudioManager.Instance.StopVoice(audioClip);
            SimpleMessageManager.Instance.HideText();
            yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Messages/Simple Message";
        private const string NODE_TITLE = "Show message: {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spAudioClip;
        private SerializedProperty spMessage;
        private SerializedProperty spColor;
        private SerializedProperty spTime;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                message.content.Length > 23
                    ? message.content.Substring(0, 20) + "..."
                    : message.content
            );
        }

        protected override void OnEnableEditorChild()
        {
            spAudioClip = serializedObject.FindProperty("audioClip");
            spMessage = serializedObject.FindProperty("message");
            spColor = serializedObject.FindProperty("color");
            spTime = serializedObject.FindProperty("time");
        }

        protected override void OnDisableEditorChild()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spMessage);
            EditorGUILayout.PropertyField(spAudioClip);
            EditorGUILayout.PropertyField(spColor);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spTime);

            if (spAudioClip.objectReferenceValue != null)
            {
                AudioClip clip = (AudioClip) spAudioClip.objectReferenceValue;
                if (!Mathf.Approximately(clip.length, spTime.floatValue))
                {
                    Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniButton);
                    btnRect = new Rect(
                        btnRect.x + EditorGUIUtility.labelWidth,
                        btnRect.y,
                        btnRect.width - EditorGUIUtility.labelWidth,
                        btnRect.height
                    );

                    if (GUI.Button(btnRect, "Use Audio Length", EditorStyles.miniButton))
                    {
                        spTime.floatValue = clip.length;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}