using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace LowPolyHnS.Characters
{
    public class CharacterAnimation
    {
        public enum Layer
        {
            Layer1,
            Layer2,
            Layer3
        }

        public const float EPSILON = 0.01f;

        private const string GRAPH_NAME = "Character Graph";
        private const string ERR_NORTC = "No Runtime Controller found in Character Animation";

        // PROPERTIES: ----------------------------------------------------------------------------

        private CharacterAnimator characterAnimator;
        private RuntimeAnimatorController runtimeController;

        private PlayableGraph graph;
        private AnimatorControllerPlayable runtimeControllerPlayable;

        private AnimationMixerPlayable mixerGesturesOutput;
        private AnimationMixerPlayable mixerGesturesInput;
        private AnimationMixerPlayable mixerStatesOutput;
        private AnimationMixerPlayable mixerStatesInput;

        private List<PlayableGesture> gestures;
        private List<PlayableState> states;

        // INITIALIZERS: --------------------------------------------------------------------------

        public CharacterAnimation(CharacterAnimator characterAnimator, CharacterState defaultState = null)
        {
            this.characterAnimator = characterAnimator;
            runtimeController = defaultState != null
                ? defaultState.GetRuntimeAnimatorController()
                : characterAnimator.animator.runtimeAnimatorController;

            Setup();
        }

        public void OnDestroy()
        {
            if (!graph.Equals(null)) graph.Destroy();
        }

        public void ChangeRuntimeController(RuntimeAnimatorController controller = null)
        {
            if (controller != null) runtimeController = controller;
            Setup();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Update()
        {
            for (int i = states.Count - 1; i >= 0; --i)
            {
                bool remove = states[i].Update();
                if (remove)
                {
                    states[i].Destroy();
                    states.RemoveAt(i);
                }
            }

            for (int i = gestures.Count - 1; i >= 0; --i)
            {
                bool remove = gestures[i].Update();
                if (remove)
                {
                    gestures[i].Destroy();
                    gestures.RemoveAt(i);
                }
            }
        }

        // GESTURE METHODS: -----------------------------------------------------------------------

        public void PlayGesture(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
        {
            StopGesture(fadeIn);
            gestures.Add(PlayableGesture.Create(
                animationClip, avatarMask,
                fadeIn, fadeOut, speed,
                ref graph,
                ref mixerGesturesInput,
                ref mixerGesturesOutput
            ));
        }

        public void CrossFadeGesture(AnimationClip animationClip, AvatarMask avatarMask,
            float fadeIn, float fadeOut, float speed)
        {
            if (gestures.Count == 0)
            {
                gestures.Add(PlayableGesture.Create(
                    animationClip, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref graph,
                    ref mixerGesturesInput,
                    ref mixerGesturesOutput
                ));
            }
            else
            {
                PlayableGesture previous = gestures[gestures.Count - 1];
                previous.StretchDuration(fadeIn);

                gestures.Add(PlayableGesture.CreateAfter(
                    animationClip, avatarMask,
                    fadeIn, fadeOut, speed,
                    ref graph,
                    previous
                ));
            }
        }

        public void StopGesture(float fadeOut)
        {
            for (int i = gestures.Count - 1; i >= 0; --i)
            {
                gestures[i].Stop(fadeOut);
            }
        }

        // STATE METHODS: -------------------------------------------------------------------------

        public void SetState(AnimationClip animationClip, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                states.Add(PlayableStateClip.Create(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    ref mixerStatesInput,
                    ref mixerStatesOutput
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                states.Insert(insertIndex, PlayableStateClip.CreateAfter(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    prevPlayable
                ));
            }
            else if (nextPlayable != null)
            {
                states.Insert(insertIndex, PlayableStateClip.CreateBefore(
                    animationClip, avatarMask, layer, 0f,
                    transition, speed, weight,
                    ref graph,
                    nextPlayable
                ));
            }
        }

        public void SetState(CharacterState stateAsset, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                states.Add(PlayableStateCharacter.Create(
                    stateAsset, avatarMask, this, layer,
                    runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref graph,
                    ref mixerStatesInput,
                    ref mixerStatesOutput
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                states.Insert(insertIndex, PlayableStateCharacter.CreateAfter(
                    stateAsset, avatarMask, this, layer,
                    runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref graph,
                    prevPlayable
                ));
            }
            else if (nextPlayable != null)
            {
                states.Insert(insertIndex, PlayableStateCharacter.CreateBefore(
                    stateAsset, avatarMask, this, layer,
                    runtimeControllerPlayable.GetTime(),
                    transition, speed, weight,
                    ref graph,
                    nextPlayable
                ));
            }
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
            float weight, float transition, float speed, int layer, bool syncTime)
        {
            PlayableState prevPlayable;
            PlayableState nextPlayable;

            int insertIndex = GetSurroundingStates(layer,
                out prevPlayable,
                out nextPlayable
            );

            if (prevPlayable == null && nextPlayable == null)
            {
                states.Add(PlayableStateRTC.Create(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    ref mixerStatesInput,
                    ref mixerStatesOutput
                ));
            }
            else if (prevPlayable != null)
            {
                if (prevPlayable.Layer == layer)
                {
                    prevPlayable.StretchDuration(transition);
                }

                states.Insert(insertIndex, PlayableStateRTC.CreateAfter(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    prevPlayable
                ));
            }
            else if (nextPlayable != null)
            {
                states.Insert(insertIndex, PlayableStateRTC.CreateBefore(
                    rtc, avatarMask, layer,
                    syncTime ? runtimeControllerPlayable.GetTime() : 0f,
                    transition, speed, weight,
                    ref graph,
                    nextPlayable
                ));
            }
        }

        public void ResetState(float time, int layer)
        {
            for (int i = 0; i < states.Count; ++i)
            {
                if (states[i].Layer == layer)
                {
                    states[i].OnExitState();
                    states[i].Stop(time);
                }
            }
        }

        public void ChangeStateWeight(int layer, float weight)
        {
            for (int i = states.Count - 1; i >= 0; --i)
            {
                if (states[i].Layer == layer)
                {
                    states[i].SetWeight(weight);
                }
            }
        }

        public CharacterState GetState(int layer)
        {
            for (int i = states.Count - 1; i >= 0; --i)
            {
                if (states[i].Layer == layer)
                {
                    return states[i].CharacterState;
                }
            }

            return null;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Setup()
        {
            if (!runtimeController) throw new Exception(ERR_NORTC);

            if (characterAnimator.animator.playableGraph.IsValid())
            {
                characterAnimator.animator.playableGraph.Destroy();
            }

            if (graph.IsValid()) graph.Destroy();

            graph = PlayableGraph.Create(GRAPH_NAME);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(
                graph, GRAPH_NAME,
                characterAnimator.animator
            );

            SetupSectionDefaultStates();
            SetupSectionStates();
            SetupSectionGestures();

            output.SetSourcePlayable(mixerGesturesOutput);

            // output.SetSourceOutputPort(0);

            graph.Play();
        }

        private void SetupSectionDefaultStates()
        {
            runtimeControllerPlayable = AnimatorControllerPlayable.Create(
                graph,
                runtimeController
            );

            mixerStatesInput = AnimationMixerPlayable.Create(graph, 1, true);
            mixerStatesInput.ConnectInput(0, runtimeControllerPlayable, 0, 1f);
            mixerStatesInput.SetInputWeight(0, 1f);
        }

        private void SetupSectionStates()
        {
            states = new List<PlayableState>();

            mixerStatesOutput = AnimationMixerPlayable.Create(graph, 1, true);
            mixerStatesOutput.ConnectInput(0, mixerStatesInput, 0, 1f);
            mixerStatesOutput.SetInputWeight(0, 1f);
        }

        private void SetupSectionGestures()
        {
            gestures = new List<PlayableGesture>();

            mixerGesturesInput = AnimationMixerPlayable.Create(graph, 1, true);
            mixerGesturesInput.ConnectInput(0, mixerStatesOutput, 0, 1f);
            mixerGesturesInput.SetInputWeight(0, 1f);

            mixerGesturesOutput = AnimationMixerPlayable.Create(graph, 1, true);
            mixerGesturesOutput.ConnectInput(0, mixerGesturesInput, 0, 1f);
            mixerGesturesOutput.SetInputWeight(0, 1f);
        }

        private int GetSurroundingStates(int layer, out PlayableState prev, out PlayableState next)
        {
            prev = null;
            next = null;

            for (int i = 0; i < states.Count; ++i)
            {
                if (states[i].Layer <= layer)
                {
                    prev = states[i];
                    return i;
                }

                next = states[i];
            }

            return 0;
        }
    }
}