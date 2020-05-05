using UnityEditor;

namespace LowPolyHnS.Core
{
    public abstract class IDatabaseEditor : Editor
    {
        // ABSTRACT METHODS: ----------------------------------------------------------------------

        public abstract string GetName();

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual void OnPreferencesWindowGUI()
        {
            OnInspectorGUI();
        }

        public virtual int GetPanelWeight()
        {
            return 50;
        }

        public virtual bool CanBeDecoupled()
        {
            return false;
        }
    }
}