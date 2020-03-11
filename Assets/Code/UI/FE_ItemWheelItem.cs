using UnityEngine;
using UnityEngine.UI;

public class FE_ItemWheelItem : MonoBehaviour
{
     public FE_Item RepresentedItem = null;
    [Space(5f)]
    [SerializeField] Text itemName = null;
    [SerializeField] Text itemUses = null;

    public void SetItem(FE_Item _newItem)
    {
        RepresentedItem = _newItem;
        itemName.text = _newItem.ItemName;

        if(_newItem is FE_Weapon)
        {
            FE_Weapon _wep = _newItem as FE_Weapon;

            itemUses.text = $"{_wep.MagAmmoLeft} / {FE_PlayerInventoryInteraction.Instance.GetCurrentAmmoCount(_wep.AmmoType)}";
        }
        else
        {
            itemUses.text = _newItem.ItemUses.ToString();
        }
    }
}
