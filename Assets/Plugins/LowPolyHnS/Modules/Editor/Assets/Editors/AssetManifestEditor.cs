using UnityEditor;

namespace LowPolyHnS.ModuleManager
{
    [CustomEditor(typeof(AssetManifest))]
    public class AssetManifestEditor : Editor
    {
        private const string MODULE = "{0} - {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private AssetManifest manifest;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            manifest = (AssetManifest) target;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            for (int i = 0; i < manifest.manifests.Length; ++i)
            {
                string module = string.Format(
                    MODULE,
                    manifest.manifests[i].module.moduleID,
                    manifest.manifests[i].module.version
                );

                EditorGUILayout.HelpBox(module, MessageType.None);
            }
        }
    }
}