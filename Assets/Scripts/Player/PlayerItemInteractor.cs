using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerItemInteractor : NetworkBehaviour
{
    public List<GameObject> itemsTouching;
    void Start()
    {
        if (!IsOwner) return;
        itemsTouching = new List<GameObject>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleInteractions();
        DetermineInteractableItem();
    }

    [SerializeField] private ItemClass interactableItem;

    private void DetermineInteractableItem()
    {
        if (itemsTouching.Count > 0)
        {
            // I hate this but I can't think of another easy solution
            float minDist = 1000000000000000000;
            float currDist;

            for (int i = 0; i < itemsTouching.Count; i++)
            {
                if (itemsTouching[i].GetComponent<ItemClass>().interactable == false) continue;

                Transform itemObject = itemsTouching[i].transform.GetChild(0);

                currDist = Vector3.Distance(itemObject.position, transform.position);

                if (currDist <= minDist)
                {
                    minDist = currDist;
                    interactableItem = itemsTouching[i].GetComponent<ItemClass>();
                }
            }

        } 
        else
        {
            interactableItem = null;
        }
    }

    private void HandleInteractions()
    {
        // If player is touching items
        if (interactableItem != null)
        {
            if (Input.GetButton("Interact"))
            {
                ItemClass item = interactableItem;
                interactableItem = null;
                Debug.Log("Picked up " + item.itemName);
                item.pickedUp = true;
                item.interactable = false;
                item.PickUpItemServerRpc();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item")
        {
            if (!itemsTouching.Contains(other.gameObject))
            {
                itemsTouching.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item")
        {
            if (itemsTouching.Contains(other.gameObject))
            {
                if (interactableItem == other.gameObject.GetComponent<ItemClass>()) interactableItem = null;
                itemsTouching.Remove(other.gameObject);
            }
        }
    }
}
