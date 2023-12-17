using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.VisualScripting;

public class SessionData : MonoBehaviour
{
    public Lobby lobby;
    public string errorStatus; // Should remain empty, if not, then error has occurred
    public string relayJoinCode;
    public PlayerSessionDatabase playerSessionDatabase;

    private void Start()
    {
        transform.SetParent(SessionInterface.Instance.transform);
    }
}
