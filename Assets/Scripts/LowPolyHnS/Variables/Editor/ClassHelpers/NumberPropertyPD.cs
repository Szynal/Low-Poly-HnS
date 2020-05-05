using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(NumberProperty))]
    public class NumberPropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int) Variable.DataType.Number;
        }
    }
}