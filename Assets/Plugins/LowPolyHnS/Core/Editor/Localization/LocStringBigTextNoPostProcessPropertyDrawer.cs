using LowPolyHnS.Core;
using UnityEditor;

namespace LowPolyHnS.Localization
{
    [CustomPropertyDrawer(typeof(LocStringBigTextNoPostProcessAttribute))]
    public class LocStringBigTextNoPostProcessPropertyDrawer : LocStringBigTextPropertyDrawer
    {
        protected override bool PaintPostProcess()
        {
            return false;
        }
    }
}