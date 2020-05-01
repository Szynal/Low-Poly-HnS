using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionNearestInLayer : IActionNearest
    {
        public LayerMask layerMask = -1;

        protected override int FilterLayerMask()
        {
            return layerMask;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Nearest in LayerMask";
        private const string NODE_TITLE = "Get nearest object in LayerMask";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

#endif
    }
}