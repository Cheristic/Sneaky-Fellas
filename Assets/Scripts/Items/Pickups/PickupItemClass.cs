using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickupItemClass : ItemClass
{
    public override void PickUp()
    {
        GameObject player = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)NetworkManager.Singleton.LocalClientId];
        player.GetComponentInChildren<ItemSlotManager>().secondaryPickupSlot = this;
    }
    public override void Use()
    {

    }
}
