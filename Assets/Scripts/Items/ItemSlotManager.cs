using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSlotManager : MonoBehaviour
{
    public GameObject primaryWeaponSlot;

    public GameObject secondaryPickupSlot;

    public ItemClass weaponInstance;

    public ItemClass pickupInstance;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponInstance = null;
        pickupInstance = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PickUpItemServerRpc(ItemClass item, ServerRpcParams serverRpcParams = default)
    {
        item.clientOwnerId = serverRpcParams.Receive.SenderClientId;
        item.GetComponent<NetworkObject>().ChangeOwnership(item.clientOwnerId);
        if (item.GetType().IsSubclassOf(typeof(WeaponItemClass))) 
        {
            weaponInstance = item;
        } else
        {
            pickupInstance = item;
        }
    }

    private void Update()
    {
        if (weaponInstance != null)
        {
            weaponInstance.transform.position = rb.transform.position;
            weaponInstance.transform.rotation = rb.transform.rotation;
        }
    }
}
