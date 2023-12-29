using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles when input for throw is initiated and when it's performed.
/// </summary>
public class ThrowItem_InputHandler
{

    private bool isThrowing = false;
    PlayerInputAction inputs;
    public ThrowItem_InputHandler(ref PlayerInputAction input, ref GameObject primaryWeaponSlot, ref GameObject secondaryPickupSlot)
    {
        inputs = input;
        inputs.MainPlayer.ThrowPrimary.started += InitiateThrow_Primary;
        inputs.MainPlayer.ThrowSecondary.started += InitiateThrow_Secondary;
        inputs.MainPlayer.ThrowPrimary.performed += ThrowItem;
        inputs.MainPlayer.ThrowSecondary.performed += ThrowItem;
    }

    GameObject primaryWeaponSlot;
    GameObject secondaryPickUpSlot;

    ItemClass itemToThrow;

    // The INITIATE THROW methods prepare the current item to be thrown then accessed by ThrowItem()
    public void InitiateThrow_Primary(InputAction.CallbackContext obj)
    {
        itemToThrow = primaryWeaponSlot.GetComponent<ItemClass>();
    }

    public void InitiateThrow_Secondary(InputAction.CallbackContext obj)
    {
        itemToThrow = secondaryPickUpSlot.GetComponent<ItemClass>();
    }

    /// <summary>
    /// Called once throw is performed.
    /// </summary>
    public void ThrowItem(InputAction.CallbackContext obj)
    {

    }
}
