using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameManager : NetworkBehaviour
{
    public static InGameManager Instance { get; private set; }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {

            NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("MainMenu"));
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenu"));
            SceneManager.SetActiveScene(gameObject.scene);
            PlayerManager.Instance.SpawnPlayersServerRpc();
        }
    }
}
