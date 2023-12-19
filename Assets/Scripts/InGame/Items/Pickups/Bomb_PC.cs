using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_PC : PickupItemClass
{
    void Start()
    {
        itemName = "Handgun";
        if (!IsOwner)
        {
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("BehindMask");
            return;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
    }

    public override void Use()
    {

    }
}
