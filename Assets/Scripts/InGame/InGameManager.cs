using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameManager : NetworkBehaviour
{
    public static InGameManager Instance { get; private set; }


    public void Start()
    {
        if (IsServer)
        {
            PlayerSpawnManager.Instance.SpawnPlayersServerRpc();
        }
    }
}
