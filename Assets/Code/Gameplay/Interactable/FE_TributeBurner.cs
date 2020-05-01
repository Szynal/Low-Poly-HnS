using LowPolyHnS;
using UnityEngine;

public class FE_TributeBurner : FE_InteractableObject
{
    [Header("Properties native to TributeBurner")] [SerializeField]
    private bool canBurnImportantThings = false;

    [Space(10f)] [SerializeField] private FE_MultipleStateObject gateMSO = null;
    [SerializeField] private FE_MultipleStateObject burnedBranchMSO = null;
    [SerializeField] private FE_MultipleStateObject keyMSO = null;
    [Space(10f)] [SerializeField] private CutsceneController gateCutscene = null;
    [SerializeField] private CutsceneController branchCutscene = null;
    [SerializeField] private CutsceneController keyCutscene = null;

    public override void Activate(FE_PlayerInventoryInteraction _instigator, FE_Item _itemUsed = null,
        bool _bypassItemUsage = false)
    {
        if (_itemUsed != null)
        {
            handleBurningObject(_itemUsed as FE_UseableItem);
        }
    }

    private void handleBurningObject(FE_UseableItem _burnedItem)
    {
        if (_burnedItem == null)
        {
            return;
        }

        FE_PlayerInventoryInteraction _playerInv = FE_PlayerInventoryInteraction.Instance;
        bool _burnedTheItem = false;

        switch (_burnedItem.itemID)
        {
            case 401:
                if (canBurnImportantThings)
                {
                    gateCutscene?.PlayCutscene(FE_PlayerInventoryInteraction.Instance);
                    gateMSO?.ChangeStateByID(1, true);
                    _burnedTheItem = true;
                }

                break;

            case 404:
                if (canBurnImportantThings)
                {
                    branchCutscene?.PlayCutscene(FE_PlayerInventoryInteraction.Instance);
                    burnedBranchMSO?.ChangeStateByID(1, true);
                    _burnedTheItem = true;
                }

                break;

            case 407:
                if (canBurnImportantThings)
                {
                    keyCutscene?.PlayCutscene(FE_PlayerInventoryInteraction.Instance);
                    keyMSO?.ChangeStateByID(1, true);
                    _burnedTheItem = true;
                }

                break;

            case 416:
                if (canBurnImportantThings)
                {
                    _burnedTheItem = true;
                }

                break;

            case 491:
                for (int i = 0; i < _burnedItem.ItemUses; i++)
                {
                    _playerInv.AddAmmo(EAmmoType.Pistol, 10);
                }

                _burnedTheItem = true;
                break;

            case 492:
                for (int i = 0; i < _burnedItem.ItemUses; i++)
                {
                    _playerInv.AddAmmo(EAmmoType.MachinePistol, 30);
                }

                _burnedTheItem = true;
                break;

            case 493:
                for (int i = 0; i < _burnedItem.ItemUses; i++)
                {
                    _playerInv.AddAmmo(EAmmoType.Rifle, 20);
                }

                _burnedTheItem = true;
                break;

            case 494:
                for (int i = 0; i < _burnedItem.ItemUses; i++)
                {
                    _playerInv.AddItem(Instantiate(GameManager.Instance.ItemDatabase.GetItemByID(1007)));
                }

                _burnedTheItem = true;
                break;

            case 495:
                for (int i = 0; i < _burnedItem.ItemUses; i++)
                {
                    _playerInv.AddItem(Instantiate(GameManager.Instance.ItemDatabase.GetItemByID(1004)));
                }

                _burnedTheItem = true;
                break;
        }

        if (_burnedTheItem)
        {
            _playerInv.RemoveItem(_burnedItem);
        }
    }
}