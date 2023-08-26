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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !pickedUp)
        {
            Debug.Log("Can pick up");
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Picked up");
                pickedUp = true;
                playerAttached = other.gameObject;
                GetComponent<CircleCollider2D>().enabled = false;
                other.gameObject.GetComponent<ItemSlotManager>().PickUpItemServerRpc(this);
            }
        }
    }


    public abstract void Use();
}
