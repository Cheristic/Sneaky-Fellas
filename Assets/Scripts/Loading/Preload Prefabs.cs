using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PreloadPrefabs : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    void Start()
    {
        // Eventually likely change to Addressables
        foreach (var prefab in prefabs)
        {
            NetworkManager.Singleton.AddNetworkPrefab(prefab);
        }
        
    }
}
