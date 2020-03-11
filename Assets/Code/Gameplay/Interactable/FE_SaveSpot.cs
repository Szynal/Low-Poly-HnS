public class FE_SaveSpot : FE_InteractableObject
{
    protected override void onActivation(FE_PlayerInventoryInteraction _instigator)
    {
        base.onActivation(_instigator);

        FE_UIController.Instance.ShowSaveScreen();

        onInteractionEnd();
    }
}
