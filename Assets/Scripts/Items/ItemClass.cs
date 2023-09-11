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
    public ulong clientOwnerId;
    public NetworkObject playerAttached;

    void Start()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pickedUp ? holdingSprite : droppedSprite;
    }

    //Handle pick up collisions
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            if (Input.GetButton("Jump") && NetworkManager.Singleton.LocalClientId == (ulong)PlayerSpawnManager.Instance.GetIdByPlayerObject(other.gameObject))
            {
                Debug.Log("Picked up " + itemName);
                pickedUp = true;
                PickUpItemServerRpc();
                //transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Items");

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpItemServerRpc(ServerRpcParams serverRpcParams = default)
    {
        clientOwnerId = serverRpcParams.Receive.SenderClientId;
        GetComponent<NetworkObject>().ChangeOwnership(clientOwnerId);
        
        PickUpItemClientRpc(clientOwnerId);
    }

    [ClientRpc]
    private void PickUpItemClientRpc(ulong clientId)
    {
        clientOwnerId = clientId;
        playerAttached = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)clientOwnerId];

        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = holdingSprite;

        pickedUp = true;
        GetComponent<CircleCollider2D>().enabled = false;
        var p = playerAttached.GetComponentInChildren<ItemSlotManager>();

        if (GetType().IsSubclassOf(typeof(WeaponItemClass)))
        {
            if (p.weaponInstance) DropItemServerRpc("weapon");
            p.weaponInstance = this;

        }
        else
        {
            if (p.pickupInstance) DropItemServerRpc("pickup");
            p.pickupInstance = this;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(string itemType, ServerRpcParams serverRpcParams = default)
    {
        clientOwnerId = serverRpcParams.Receive.SenderClientId;

        var p = playerAttached.GetComponentInChildren<ItemSlotManager>();
        if (itemType == "weapon") p.weaponInstance.GetComponent<NetworkObject>().RemoveOwnership();
        else p.pickupInstance.GetComponent<NetworkObject>().RemoveOwnership();

        DropItemClientRpc(itemType);
    }

    [ClientRpc]
    private void DropItemClientRpc(string itemType)
    {
        var itemSlots = playerAttached.GetComponentInChildren<ItemSlotManager>();

        ItemClass itemToDrop = itemType == "weapon" ? itemSlots.weaponInstance : itemSlots.pickupInstance;
        itemToDrop.GetComponentInChildren<SpriteRenderer>().sprite = itemToDrop.droppedSprite;

        itemToDrop.pickedUp = false;
        itemToDrop.GetComponent<CircleCollider2D>().enabled = true;

        if (itemType == "weapon") itemSlots.weaponInstance = null;
        else itemSlots.pickupInstance = null;
    }

    public abstract void Use();

}
