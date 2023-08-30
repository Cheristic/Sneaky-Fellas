using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameManager : NetworkBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField] private GameObject cameraObject;


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        if (IsServer)
        {
            //StartCoroutine(DelaySpawn());
            PlayerSpawnManager.Instance.SpawnPlayersServerRpc();
        }
      
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1f);
        PlayerSpawnManager.Instance.SpawnPlayersServerRpc();

    }
}
