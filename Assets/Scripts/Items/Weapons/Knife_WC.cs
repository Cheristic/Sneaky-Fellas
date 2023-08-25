using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife_WC : WeaponItemClass
{
    private void Start()
    {
        itemName = "Knife";
    }
    public override void Use()
    {
        base.Use();
        GetComponentInChildren<KnifeWeaponParent>().PlayerAttack();

    }
}
