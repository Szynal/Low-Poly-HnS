using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class IgniterDropItem : IgniterDropToConsume
    {
#if UNITY_EDITOR
        public new static string NAME = "Inventory/On Drop Item";
        public new static bool REQUIRES_COLLIDER = true;
        public new static string ICON_PATH = "Assets/Content/Icons/Inventory/Igniters/";
#endif

        public ItemHolder item;

        public override void OnDrop(Item item)
        {
            if (item != null && item.uuid == this.item.item.uuid) ExecuteTrigger();
        }
    }
}