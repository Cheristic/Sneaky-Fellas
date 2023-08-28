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
    public GameObject playerAttached;

    //Handle pick up collisions
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            if (Input.GetButton("Jump"))
            {
                Debug.Log("Picked up " + itemName);
                pickedUp = true;
                playerAttached = other.gameObject;
                GetComponent<CircleCollider2D>().enabled = false;
                other.gameObject.GetComponent<ItemSlotManager>().PickUpItemServerRpc(this);
            }
        }
    }


    public abstract void Use();

}
