using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class DatabaseVariables : IDatabase
    {
        [Serializable]
        public class Container
        {
            public List<Variable> variables;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        [SerializeField] protected GlobalTags tags;
        [SerializeField] protected GlobalVariables variables;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GlobalVariables GetGlobalVariables()
        {
            return variables;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseVariables Load()
        {
            return LoadDatabase<DatabaseVariables>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Setup<DatabaseVariables>();
        }

        public override int GetSidebarPriority()
        {
            return 3;
        }

#endif
    }
}