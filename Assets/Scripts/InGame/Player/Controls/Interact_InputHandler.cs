using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Determines what object, if any, the player will interact with when the "Interact" input is triggered.
/// Pings server to ensure no conflict. When server responds, item is interacted with (easier solution)
/// (Alternative is triggering interaction then backpedal when conflict arises).
/// </summary>
public class Interact_InputHandler
{

}