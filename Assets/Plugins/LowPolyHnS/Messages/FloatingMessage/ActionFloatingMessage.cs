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
    public class ActionFloatingMessage : IAction
    {
        public AudioClip audioClip;

        [LocStringNoPostProcess] public LocString message = new LocString();

        public Color color = Color.white;
        public float time = 2.0f;

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        public Vector3 offset = new Vector3(0, 2, 0);

        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTransform = this.target.GetTransform(target);
            if (targetTransform != null)
            {
                if (audioClip != null)
                {
                    AudioMixerGroup voiceMixer = DatabaseGeneral.Load().voiceAudioMixer;
                    AudioManager.Instance.PlayVoice(audioClip, 0f, 1f, voiceMixer);
                }

                FloatingMessageManager.Show(
                    message.GetText(), color,
                    targetTransform, offset, time
                );

                float waitTime = Time.time + time;
                WaitUntil waitUntil = new WaitUntil(() => Time.time > waitTime || forceStop);
                yield return waitUntil;

                if (audioClip != null)
                {
                    AudioManager.Instance.StopVoice(audioClip);
                }
            }

            yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
        }

#if UNITY_EDITOR
        public static new string NAME = "Messages/Floating Message";
        private const string NODE_TITLE = "Show message: {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spAudioClip;
        private SerializedProperty spMessage;
        private SerializedProperty spColor;
        private SerializedProperty spTime;
        private SerializedProperty spTarget;
        private SerializedProperty spOffset;

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

            spTarget = serializedObject.FindProperty("target");
            spOffset = serializedObject.FindProperty("offset");
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

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.PropertyField(spOffset);

            serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}