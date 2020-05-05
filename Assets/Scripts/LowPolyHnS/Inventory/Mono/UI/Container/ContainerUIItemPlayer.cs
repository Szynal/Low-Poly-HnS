namespace LowPolyHnS.Inventory
{
    public class ContainerUIItemPlayer : IContainerUIItem
    {
        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public override void Setup(ContainerUIManager containerUIManager, object item)
        {
            base.Setup(containerUIManager, item);
            this.item = item as Item;

            UpdateUI();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        protected override int GetAmount()
        {
            return InventoryManager.Instance.GetInventoryAmountOfItem(item.uuid);
        }

        public override void OnClickButton()
        {
            Container container = containerUIManager.currentContainer;

            int subtract = InventoryManager.Instance.SubstractItemFromInventory(item.uuid);
            if (subtract <= 0) return;

            container.AddItem(item.uuid, subtract);

            containerUIManager.UpdateItems();
            if (containerUIManager.onAdd != null)
            {
                containerUIManager.onAdd.Invoke(item.uuid);
            }
        }
    }
}