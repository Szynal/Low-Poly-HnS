using UnityEditor;

namespace LowPolyHnS.Core
{
    public abstract class HotspotSubEditors
    {
        // COMMON PAINT METHOD: ----------------------------------------------------------------------------------------

        private static void PaintMessage()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("These are not the Components you are looking for", MessageType.Warning);
            EditorGUILayout.Space();
        }

        [CustomEditor(typeof(HPCursor))]
        public class HPCursorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                PaintMessage();
            }
        }

        [CustomEditor(typeof(HPProximity))]
        public class HPProximityEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                PaintMessage();
            }
        }
    }
}