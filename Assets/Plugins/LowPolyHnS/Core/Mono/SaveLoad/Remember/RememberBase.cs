﻿using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public abstract class RememberBase : GlobalID, IGameSave
    {
        protected bool isDestroyed;

        // IGAMESAVE: -----------------------------------------------------------------------------

        public abstract object GetSaveData();
        public abstract Type GetSaveDataType();

        public abstract string GetUniqueName();

        public abstract void OnLoad(object generic);
        public abstract void ResetData();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void Start()
        {
            if (!Application.isPlaying || exitingApplication) return;
            SaveLoadManager.Instance.Initialize(this);
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying || exitingApplication) return;

            isDestroyed = true;
            SaveLoadManager.Instance.OnDestroyIGameSave(this);
        }
    }
}