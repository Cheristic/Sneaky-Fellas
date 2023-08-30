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
    public Sprite pickedUpSprite;
    public bool pickedUp = false;
    public ulong clientOwnerId;
    public NetworkObject playerAttached;

    //Handle pick up collisions
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            if (Input.GetButton("Jump"))
            {
                Debug.Log("Picked up " + itemName);
                pickedUp = true;
                /*playerAttached = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)clientOwnerId];
                pickedUp = true;
                GetComponent<CircleCollider2D>().enabled = false;

                if (GetType().IsSubclassOf(typeof(WeaponItemClass)))
                {
                    playerAttached.GetComponent<ItemSlotManager>().weaponInstance = this;
                }
                else
                {
                    playerAttached.GetComponent<ItemSlotManager>().pickupInstance = this;
                }*/
                PickUpItemServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpItemServerRpc(ServerRpcParams serverRpcParams = default)
    {
        clientOwnerId = serverRpcParams.Receive.SenderClientId;
        Debug.Log("Id is " + clientOwnerId);
        GetComponent<NetworkObject>().ChangeOwnership(clientOwnerId);
        
        PickUpItemClientRpc(clientOwnerId);
    }

    [ClientRpc]
    private void PickUpItemClientRpc(ulong clientId)
    {
        //if (NetworkManager.Singleton.LocalClientId == clientOwnerId) return;
        clientOwnerId = clientId;
        playerAttached = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)clientOwnerId];
        Debug.Log("Player is " + playerAttached.OwnerClientId);
        pickedUp = true;
        GetComponent<CircleCollider2D>().enabled = false;

        var p = playerAttached.GetComponentInChildren<ItemSlotManager>();
        if (GetType().IsSubclassOf(typeof(WeaponItemClass)))
        {
            p.weaponInstance = this;
            //p.weaponInstance.GetComponent<NetworkObject>().TrySetParent(p.primaryWeaponSlot.transform, false);

        }
        else
        {
            p.pickupInstance = this;
            //p.pickupInstance.GetComponent<NetworkObject>().TrySetParent(p.secondaryPickupSlot.transform, false);
        }

    }

    public abstract void Use();

}
