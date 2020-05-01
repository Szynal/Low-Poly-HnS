namespace LowPolyHnS.Inventory
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using LowPolyHnS.Core;

    [CustomEditor(typeof(PlayerEquipment))]
    public class PlayerEquipmentEditor : CharacterEquipmentEditor
    {
        protected override bool PaintGlobalID()
        {
            return false;
        }
    }
}