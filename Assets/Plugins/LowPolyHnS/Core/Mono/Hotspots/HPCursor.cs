using System;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class HPCursor : IHPMonoBehaviour<HPCursor.Data>
    {
        [Serializable]
        public class Data : IData
        {
            public Texture2D mouseOverCursor = null;
            public Vector2 cursorPosition = Vector2.zero;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void Initialize()
        {
            if (!data.enabled) return;
        }

        // HOVER CURSOR: --------------------------------------------------------------------------

        public override void HotspotMouseEnter()
        {
            if (data.enabled && data.mouseOverCursor != null)
            {
                Cursor.SetCursor(data.mouseOverCursor, data.cursorPosition, CursorMode.Auto);
            }
        }

        public override void HotspotMouseExit()
        {
            if (data.enabled && data.mouseOverCursor != null)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        public override void HotspotMouseOver()
        {
            if (data.enabled && data.mouseOverCursor != null)
            {
                if (IsWithinConstrainedRadius()) HotspotMouseEnter();
                else HotspotMouseExit();
            }
        }
    }
}