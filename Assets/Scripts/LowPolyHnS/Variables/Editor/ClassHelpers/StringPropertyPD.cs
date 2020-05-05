using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(StringProperty))]
    public class StringPropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int) Variable.DataType.String;
        }
    }
}