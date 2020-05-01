using UnityEditor;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(RememberActive))]
    public class RememberActiveEditor : RememberEditor
    {
        protected override string Comment()
        {
            return "Automatically restores the state (active, inactive or destroyed) when loading a game";
        }

        protected override void OnPaint()
        {
        }
    }
}