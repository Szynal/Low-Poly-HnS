﻿using System.Collections;
using System.Collections.Generic;
using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace LowPolyHnS.Messages
{
    public static class FloatingMessageManager
    {
        private const string CANVAS_ASSET_PATH = "LowPolyHnS/Messages/FloatingMessage";
        private const float TRANSITION_TIME = 0.3f;
        private static int ANIMATOR_HASH_CLOSE = -1;

        private static bool INITIALIZED = false;
        private static GameObject PREFAB;

        // INITIALIZE: ----------------------------------------------------------------------------

        private static void RequireInit()
        {
            if (INITIALIZED) return;
            EventSystemManager.Instance.Wakeup();
            ANIMATOR_HASH_CLOSE = Animator.StringToHash("Close");

            DatabaseGeneral general = DatabaseGeneral.Load();
            PREFAB = general.prefabFloatingMessage;
            if (PREFAB == null) PREFAB = Resources.Load<GameObject>(CANVAS_ASSET_PATH);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Show(string message, Color color, Transform target, Vector3 offset, float duration)
        {
            RequireInit();
            GameObject instance = Object.Instantiate(PREFAB, target);
            instance.transform.localPosition = offset;

            CoroutinesManager.Instance.StartCoroutine(CoroutineShow(message, color, instance, duration));
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static IEnumerator CoroutineShow(string message, Color color,
            GameObject instance, float duration)
        {
            Canvas canvas = instance.GetComponent<Canvas>();
            Camera camera = HookCamera.Instance != null ? HookCamera.Instance.Get<Camera>() : null;
            if (!camera) camera = Object.FindObjectOfType<Camera>();

            if (canvas != null) canvas.worldCamera = camera;

            Animator animator = instance.GetComponentInChildren<Animator>();
            Text text = instance.GetComponentInChildren<Text>();

            text.text = message;
            text.color = color;

            LookAtConstraint constraint = instance.GetComponent<LookAtConstraint>();
            if (constraint != null)
            {
                constraint.SetSources(new List<ConstraintSource>
                {
                    new ConstraintSource
                    {
                        sourceTransform = HookCamera.Instance.transform,
                        weight = 1.0f
                    }
                });

                constraint.constraintActive = true;
            }

            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(duration - TRANSITION_TIME);
            yield return wait;

            if (animator != null) animator.SetTrigger(ANIMATOR_HASH_CLOSE);

            wait = new WaitForSecondsRealtime(TRANSITION_TIME);
            yield return wait;

            if (instance != null) Object.Destroy(instance);
        }
    }
}