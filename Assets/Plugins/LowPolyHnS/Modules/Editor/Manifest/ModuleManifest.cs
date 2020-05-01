using System;

namespace LowPolyHnS.ModuleManager
{
    [Serializable]
    public class ModuleManifest
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public Module module;

        // INITIALIZERS: --------------------------------------------------------------------------

        public ModuleManifest()
        {
            module = new Module();
        }

        public ModuleManifest(Module module)
        {
            this.module = module;
        }
    }
}