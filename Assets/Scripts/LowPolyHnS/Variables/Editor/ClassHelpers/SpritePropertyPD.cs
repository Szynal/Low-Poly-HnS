using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(SpriteProperty))]
    public class SpritePropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int) Variable.DataType.Sprite;
        }
    }
}