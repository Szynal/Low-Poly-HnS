namespace LowPolyHnS.Inventory
{
    public class ContainerUIItemBox : IContainerUIItem
    {
        private Container.ItemData containerData;

        // CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public override void Setup(ContainerUIManager containerUIManager, object item)
        {
            base.Setup(containerUIManager, item);
            containerData = item as Container.ItemData;
            this.item = InventoryManager.Instance.itemsCatalogue[containerData.uuid];

            UpdateUI();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        protected override int GetAmount()
        {
            return containerData.amount;
        }

        public override void OnClickButton()
        {
            Container container = containerUIManager.currentContainer;

            int addAmount = InventoryManager.Instance.AddItemToInventory(item.uuid);
            if (addAmount <= 0) return;

            container.RemoveItem(item.uuid, addAmount);

            containerUIManager.UpdateItems();
            if (containerUIManager.onRemove != null)
            {
                containerUIManager.onRemove.Invoke(item.uuid);
            }
        }
    }
}