public class FE_Torch : FE_InteractableObject
{
    public override void Activate(FE_PlayerInventoryInteraction _instigator, FE_Item _itemUsed = null,
        bool _bypassItemUsage = false)
    {
        if (canInteract)
        {
            if (_itemUsed != null)
            {
                canInteract = false;
            }
        }
    }
}