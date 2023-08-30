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
            StartCoroutine(DelaySpawn());
        }
      
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1f);
        PlayerSpawnManager.Instance.SpawnPlayersServerRpc();

    }

    [ClientRpc]
    public void StartCameraFollowClientRpc()
    {
        if (IsClient)
        {
            var id = NetworkManager.Singleton.LocalClientId;
            if (PlayerSpawnManager.Instance.networkPlayersSpawned[(int)id] is not null)
            {
                //Transform p = PlayerSpawnManager.Instance.networkPlayersSpawned[(int)id].transform.GetChild(0);
                //cameraObject.GetComponent<CameraMovement>().playerToTrack = p.GetComponent<Rigidbody2D>();
            }
        }
    }
}
