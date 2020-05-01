using UnityEngine;

[CreateAssetMenu(fileName = "NewUseable", menuName = "FearEffect/Items/UseableItem", order = 2)]
public class FE_UseableItem : FE_Item
{
    [Header("Specific for UseableItem")] public bool IsStackable = false;
    public int MaxStacks = 99;

    public override void Activate(FE_PlayerInventoryInteraction _instigator)
    {
        base.Activate(_instigator);

        FE_PlayerInventoryInteraction _interactionController =
            _instigator.GetComponent<FE_PlayerInventoryInteraction>();
        if (_interactionController.HasInteraction() && _interactionController.GetInteraction().RequireItem())
        {
            if (_interactionController.GetInteraction() is FE_TributeBurner)
            {
                (_interactionController.GetInteraction() as FE_TributeBurner).Activate(_interactionController, this);
            }
            else
            {
                _interactionController.GetInteraction().Activate(_interactionController, this);
            }
        }
    }

    public bool UseStack(int _amt = 1)
    {
        ItemUses -= -_amt;

        if (ItemUses <= 0)
        {
            return true;
        }

        return false;
    }
}