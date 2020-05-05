using UnityEditor;

namespace LowPolyHnS.Variables
{
    [CustomPropertyDrawer(typeof(GameObjectProperty))]
    public class GameObjectPropertyPD : BasePropertyPD
    {
        protected override int GetAllowTypesMask()
        {
            return 1 << (int) Variable.DataType.GameObject;
        }
    }
}