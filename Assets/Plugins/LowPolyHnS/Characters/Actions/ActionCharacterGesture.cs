using System;
using System.Collections;
using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterGesture : IAction
    {
        public TargetCharacter character = new TargetCharacter();
        public AnimationClip clip;
        public AvatarMask avatarMask;

        public NumberProperty speed = new NumberProperty(1.0f);

        public float fadeIn = 0.1f;
        public float fadeOut = 0.1f;
        public bool waitTillComplete = false;

        private CharacterAnimator characterAnimator;
        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = character.GetCharacter(target);
            if (clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                characterAnimator = charTarget.GetCharacterAnimator();
                characterAnimator.CrossFadeGesture(
                    clip, speed.GetValue(target), avatarMask,
                    fadeIn, fadeOut
                );
            }

            return !waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            forceStop = false;
            Character charTarget = character.GetCharacter(target);
            if (clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                if (waitTillComplete)
                {
                    float wait = Time.time + clip.length / speed.GetValue(target);

                    WaitUntil waitUntil = new WaitUntil(() => forceStop || Time.time > wait);
                    yield return waitUntil;
                }
            }

            yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
            if (characterAnimator == null) return;
            characterAnimator.StopGesture(fadeOut);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Gesture";
        private const string NODE_TITLE = "Character {0} do gesture {1}";

        private static readonly GUIContent GC_MASK = new GUIContent("Mask (optional)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spClip;
        private SerializedProperty spAvatarMask;
        private SerializedProperty spWaitTillComplete;
        private SerializedProperty spSpeed;
        private SerializedProperty spFadeIn;
        private SerializedProperty spFadeOut;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string clipName = clip == null ? "none" : clip.name;
            if (clipName.Contains("@"))
            {
                string[] split = clipName.Split(new[] {'@'}, 2, StringSplitOptions.RemoveEmptyEntries);
                clipName = split[split.Length - 1];
            }

            return string.Format(NODE_TITLE, character, clipName);
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spClip = serializedObject.FindProperty("clip");
            spAvatarMask = serializedObject.FindProperty("avatarMask");
            spWaitTillComplete = serializedObject.FindProperty("waitTillComplete");
            spSpeed = serializedObject.FindProperty("speed");
            spFadeIn = serializedObject.FindProperty("fadeIn");
            spFadeOut = serializedObject.FindProperty("fadeOut");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spClip = null;
            spAvatarMask = null;
            spWaitTillComplete = null;
            spSpeed = null;
            spFadeIn = null;
            spFadeOut = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spClip);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spAvatarMask, GC_MASK);
            EditorGUILayout.PropertyField(spSpeed);

            EditorGUILayout.PropertyField(spFadeIn);
            EditorGUILayout.PropertyField(spFadeOut);
            EditorGUILayout.PropertyField(spWaitTillComplete);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}