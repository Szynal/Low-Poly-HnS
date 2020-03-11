using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_Torch : FE_InteractableObject
{
    public override void Activate(FE_PlayerInventoryInteraction _instigator, FE_Item _itemUsed = null, bool _bypassItemUsage = false)
    {
        if (canInteract == true)
        {
            if (_itemUsed != null)
            {
                canInteract = false;
            }
        }
    }
}
