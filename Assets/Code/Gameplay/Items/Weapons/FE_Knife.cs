using UnityEngine;

[CreateAssetMenu(fileName = "NewKnife", menuName = "FearEffect/Items/Premade/Knife", order = 0)]
public class FE_Knife : FE_Weapon
{
    public FE_Knife()
    {
        itemID = 1000;
        ItemName = "Knife";
        Damage = 30;
        AttackRange = 1f;
    }
}