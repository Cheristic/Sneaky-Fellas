using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraMovement : NetworkBehaviour
{
    [SerializeField] private Transform playerToTrack;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private CinemachineVirtualCamera vc;

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            cameraHolder.SetActive(true);
            vc.Priority = 10;
        } else
        {          
            vc.Priority = 0;
            cameraHolder.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        cameraHolder.transform.position = new Vector3(playerToTrack.position.x, playerToTrack.position.y, cameraHolder.transform.position.z);
    }

}
