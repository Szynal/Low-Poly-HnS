using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(ColorProperty))]
    public class ColorPropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int) Variable.DataType.Color;
        }
    }
}