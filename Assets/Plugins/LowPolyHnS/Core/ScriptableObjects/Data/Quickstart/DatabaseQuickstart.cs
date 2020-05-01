namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class DatabaseQuickstart : IDatabase
    {
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseQuickstart Load()
        {
            return LoadDatabase<DatabaseQuickstart>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Setup<DatabaseQuickstart>();
        }

        public override int GetSidebarPriority()
        {
            return 0;
        }

#endif
    }
}