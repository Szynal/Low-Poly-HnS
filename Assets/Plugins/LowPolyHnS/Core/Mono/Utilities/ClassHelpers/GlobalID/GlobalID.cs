using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [ExecuteInEditMode, Serializable]
    public abstract class GlobalID : MonoBehaviour
    {
        private const int GUID_LENGTH = 16;

        // PROPERTIES: ----------------------------------------------------------------------------

        [SerializeField] private string gid;
        protected bool exitingApplication;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public string GetID()
        {
            return gid;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void OnDestroyGID()
        {
            if (exitingApplication) return;
        }

        protected virtual void OnApplicationQuit()
        {
            exitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void CreateGuid()
        {
            if (!string.IsNullOrEmpty(gid)) return;
            gid = Guid.NewGuid().ToString("D");
        }

        // INITIALIZE METHODS: --------------------------------------------------------------------

        protected virtual void Awake()
        {
            CreateGuid();
        }

        protected virtual void OnValidate()
        {
            CreateGuid();
        }
    }
}