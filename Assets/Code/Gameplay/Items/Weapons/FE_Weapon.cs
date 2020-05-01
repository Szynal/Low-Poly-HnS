using UnityEngine;

public enum EAmmoType
{
    Pistol,
    MachinePistol,
    Rifle,
    Shot
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "FearEffect/Items/Weapon", order = 1)]
public class FE_Weapon : FE_Item
{
    [Header("Properties for Weapon")] public EAmmoType AmmoType = default;
    public bool IsDualWielded = false;
    public FE_Weapon DualWieldedVariant = null;

    public int MagAmmoLeft;
    public int MagAmmoMax = 0;

    public int Damage = 0;

    public float ReloadTime = 0f;
    public float AttackCooldown = 0f;

    public float AttackRange = 0f;
    public float BackstabRange = 0f;

    public override void Activate(FE_PlayerInventoryInteraction _instigator)
    {
        base.Activate(_instigator);
    }

    public void UseAmmo()
    {
        MagAmmoLeft -= 1;
    }
}