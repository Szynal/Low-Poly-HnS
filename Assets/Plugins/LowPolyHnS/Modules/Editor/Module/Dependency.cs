using System;

namespace LowPolyHnS.ModuleManager
{
    [Serializable]
    public class Dependency
    {
        public static Dependency NONE => new Dependency("", Version.NONE);

        // PROPERTIES: ----------------------------------------------------------------------------

        public string moduleID;
        public Version version;

        // INITIALIZERS: --------------------------------------------------------------------------

        public Dependency(string name, Version version)
        {
            moduleID = name;
            this.version = version;
        }
    }
}