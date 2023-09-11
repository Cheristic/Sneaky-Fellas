using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SoundManager : NetworkBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private GameObject soundCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject[] soundSprites;

    [ServerRpc(RequireOwnership = false)]
    public void SoundCreatedServerRpc(string soundType, Vector3 soundLocation)
    {
        switch (soundType)
        {
            case "HandgunGunshot":
                SpawnSoundClientRpc(10000, soundLocation, 1);
                break;
        }
    }

    [ClientRpc]
    private void SpawnSoundClientRpc(int radius, Vector3 soundLocation, int intensity)
    {
        Transform player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.GetChild(0);
        if ((soundLocation - player.position).magnitude < radius)
        {
            var s = Instantiate(soundSprites[intensity], soundLocation, Quaternion.identity, soundCanvas.transform);
        }
    }
}
