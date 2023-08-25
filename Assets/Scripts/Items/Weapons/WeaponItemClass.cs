using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponItemClass : ItemClass
{

    public override void PickUp()
    {
        GameObject player = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)NetworkManager.Singleton.LocalClientId];
        player.GetComponentInChildren<ItemSlotManager>().primaryWeaponSlot = this;
    }
    public override void Use()
    {

    }
}
