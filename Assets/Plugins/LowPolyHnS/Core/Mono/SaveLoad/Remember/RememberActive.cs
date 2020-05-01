using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Remember/Remember Active")]
    public class RememberActive : RememberBase
    {
        public enum State
        {
            Active,
            Inactive,
            Destroyed
        }

        [Serializable]
        public class Memory
        {
            public State state;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool defaultState;
        private State state = State.Active;

        // METHODS: -------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying || exitingApplication) return;
            defaultState = gameObject.activeSelf;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying || exitingApplication) return;
            UpdateState();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying || exitingApplication) return;
            UpdateState();
        }

        private void UpdateState()
        {
            switch (gameObject.activeSelf)
            {
                case true:
                    state = State.Active;
                    break;
                case false:
                    state = State.Inactive;
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!Application.isPlaying || exitingApplication) return;
            state = State.Destroyed;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override object GetSaveData()
        {
            return new Memory
            {
                state = state
            };
        }

        public override Type GetSaveDataType()
        {
            return typeof(Memory);
        }

        public override string GetUniqueName()
        {
            return GetID();
        }

        public override void OnLoad(object generic)
        {
            Memory memory = generic as Memory;
            if (memory == null || isDestroyed) return;

            switch (memory.state)
            {
                case State.Active:
                    gameObject.SetActive(true);
                    break;
                case State.Inactive:
                    gameObject.SetActive(false);
                    break;
                case State.Destroyed:
                    Destroy(gameObject);
                    break;
            }
        }

        public override void ResetData()
        {
            gameObject.SetActive(defaultState);
        }
    }
}