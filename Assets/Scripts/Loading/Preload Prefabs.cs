using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PreloadPrefabs : MonoBehaviour
{
    [SerializeField] GameObject playerClassPrefab;
    void Start()
    {
        //var playerClassPrefab = (GameObject)Resources.Load("Assets/InGame/Player/PlayerClass");
        NetworkManager.Singleton.AddNetworkPrefab(playerClassPrefab);
    }
}
