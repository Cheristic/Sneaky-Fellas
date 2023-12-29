using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(menuName = "new Item")]
public abstract class ItemClass : NetworkBehaviour
{
    [Header("Item Attributes")]
    public string itemName;
    public Sprite droppedSprite;
    public Sprite holdingSprite;
    public bool pickedUp = false;
    public bool interactable = true;
    public ulong clientOwnerId;
    public NetworkObject playerAttached;

    void Start()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pickedUp ? holdingSprite : droppedSprite;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PickUpItemServerRpc(ServerRpcParams serverRpcParams = default)
    {
        clientOwnerId = serverRpcParams.Receive.SenderClientId;
        GetComponent<NetworkObject>().ChangeOwnership(clientOwnerId);
        
        PickUpItemClientRpc(clientOwnerId);
    }

    [ClientRpc]
    private void PickUpItemClientRpc(ulong clientId)
    {
        clientOwnerId = clientId;
        //playerAttached = PlayerSpawner.Instance.networkPlayersSpawned[(int)clientOwnerId];

        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = holdingSprite;

        pickedUp = true;
        interactable = false;
        //GetComponent<CircleCollider2D>().enabled = false;
        var p = playerAttached.GetComponentInChildren<ItemSlotManager>();

        if (GetType().IsSubclassOf(typeof(WeaponItemClass)))
        {
            if (p.weaponInstance) DropItemServerRpc("weapon", transform.position);
            p.weaponInstance = this;

        }
        else
        {
            if (p.pickupInstance) DropItemServerRpc("pickup", transform.position);
            p.pickupInstance = this;
        }

        transform.GetChild(0).localPosition = transform.GetChild(1).localPosition;

    }

    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(string itemType, Vector3 dropPoint, ServerRpcParams serverRpcParams = default)
    {
        clientOwnerId = serverRpcParams.Receive.SenderClientId;

        var p = playerAttached.GetComponentInChildren<ItemSlotManager>();
        if (itemType == "weapon") p.weaponInstance.GetComponent<NetworkObject>().RemoveOwnership();
        else p.pickupInstance.GetComponent<NetworkObject>().RemoveOwnership();

        DropItemClientRpc(itemType, dropPoint);
    }

    [ClientRpc]
    private void DropItemClientRpc(string itemType, Vector3 dropPoint)
    {
        var itemSlots = playerAttached.GetComponentInChildren<ItemSlotManager>();

        ItemClass itemToDrop = itemType == "weapon" ? itemSlots.weaponInstance : itemSlots.pickupInstance;
        itemToDrop.GetComponentInChildren<SpriteRenderer>().sprite = itemToDrop.droppedSprite;

        itemToDrop.pickedUp = false;
        itemToDrop.interactable = true;

        if (itemType == "weapon") itemSlots.weaponInstance = null;
        else itemSlots.pickupInstance = null;

        itemToDrop.transform.position = dropPoint;

        itemToDrop.transform.GetChild(0).localPosition = transform.GetChild(2).localPosition;
    }

    public abstract void Use();

}
