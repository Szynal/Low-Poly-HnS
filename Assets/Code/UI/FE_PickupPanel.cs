using UnityEngine;
using UnityEngine.UI;

public class FE_PickupPanel : MonoBehaviour
{
    [SerializeField] private Image imgItemImage = default;
    [SerializeField] private Text textItemName = default;

    private void Awake()
    {
        if (imgItemImage == null || textItemName == null)
        {
            Debug.LogError("There are unassigned fields in PickupPanel on object " + name);
        }
    }

    public void UpdateDisplay(FE_InteractableObject _newObj)
    {
        if (_newObj is FE_Pickup)
        {
            imgItemImage.sprite = ((FE_Pickup) _newObj).GetPickupItem().ItemSprite;
            textItemName.text = ((FE_Pickup) _newObj).GetPickupItem().ItemName;
        }
        else if (_newObj is FE_AmmoPickup)
        {
            imgItemImage.sprite = ((FE_AmmoPickup) _newObj).GetSprite();
            textItemName.text = ((FE_AmmoPickup) _newObj).GetAmmoType() + " ammo  -> " +
                                ((FE_AmmoPickup) _newObj).GetAmount();
        }
    }
}