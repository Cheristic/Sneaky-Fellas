using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSlotManager : NetworkBehaviour
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

    private void Update()
    {
        if (weaponInstance != null)
        {
            weaponInstance.transform.position = primaryWeaponSlot.transform.position;
            weaponInstance.transform.rotation = rb.transform.rotation;
        }
    }
}
