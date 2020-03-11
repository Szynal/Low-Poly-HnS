using UnityEngine;

[CreateAssetMenu(fileName = "NewShotgun", menuName = "FearEffect/Items/Premade/Shotgun", order = 0)]
public class FE_Shotgun : FE_Weapon
{
    [Header("Properties for Shotgun")]
    public float AttackRadius = 4f;

    public FE_Shotgun()
    {
        itemID = 1011;
        ItemName = "Shotgun";
        AmmoType = EAmmoType.Shot;
        MagAmmoLeft = 5;
        MagAmmoMax = 5;
    }
}
