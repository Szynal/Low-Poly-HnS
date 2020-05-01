using LowPolyHnS.Core;
using UnityEditor;

namespace LowPolyHnS.Localization
{
    [CustomPropertyDrawer(typeof(LocStringNoPostProcessAttribute))]
    public class LocStringNoPostProcessPropertyDrawer : LocStringPropertyDrawer
    {
        protected override bool PaintPostProcess()
        {
            return false;
        }
    }
}