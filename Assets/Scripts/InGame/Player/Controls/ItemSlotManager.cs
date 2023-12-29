using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

/// <summary>
/// Interface for managing a player's currently held items, including their use, throws, etc.
/// </summary>
public class ItemSlotManager
{
    public GameObject primaryWeaponSlot;
    public GameObject secondaryPickupSlot;

    public ItemClass weaponInstance;
    public ItemClass pickupInstance;

    PlayerInputAction inputs;
    ThrowItem_InputHandler throwItemInput;

    public ItemSlotManager(ref PlayerInputAction input)
    {
        inputs = input;
        throwItemInput = new(ref inputs, ref primaryWeaponSlot, ref secondaryPickupSlot);

        inputs.MainPlayer.UsePrimary.performed += PerformSlot;
        inputs.MainPlayer.UseSecondary.performed += PerformSlot;
    }

    private void PickUpItem()
    {

    }

    private void PerformSlot(InputAction.CallbackContext obj)
    {
        if (obj.action == inputs.MainPlayer.UsePrimary && weaponInstance != null)
        {
            weaponInstance.GetComponent<ItemClass>().Use();
        }
        if (obj.action == inputs.MainPlayer.UseSecondary && pickupInstance != null)
        {
            pickupInstance.GetComponent<ItemClass>().Use();
        }
    }

}
