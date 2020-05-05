using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor.Events;

#endif

    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Actions))]
    [AddComponentMenu("LowPolyHnS/UI/Button", 10)]
    public class ButtonActions : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class ButtonActionsEvent : UnityEvent<GameObject>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Actions actions;
        public ButtonActionsEvent onClick = new ButtonActionsEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            EventSystemManager.Instance.Wakeup();
        }

        // VALIDATE: ------------------------------------------------------------------------------

#if UNITY_EDITOR
        private new void OnValidate()
        {
            base.OnValidate();

            if (actions == null)
            {
                actions = gameObject.GetComponent<Actions>();
                if (actions == null) return;

                onClick.RemoveAllListeners();
                UnityEventTools.AddObjectPersistentListener(
                    onClick,
                    actions.ExecuteWithTarget,
                    gameObject
                );
            }

            actions.hideFlags = HideFlags.HideInInspector;
        }
#endif

        // INTERFACES: ----------------------------------------------------------------------------

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!IsActive() || !IsInteractable()) return;
            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Press()
        {
            if (!IsActive() || !IsInteractable()) return;
            if (onClick != null) onClick.Invoke(gameObject);
        }
    }
}