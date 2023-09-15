using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraMovement : NetworkBehaviour
{
    [SerializeField] private Transform targetToTrack;
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
            vc.Priority = -1;
            cameraHolder.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        cameraHolder.transform.position = new Vector3(targetToTrack.position.x, targetToTrack.position.y, cameraHolder.transform.position.z);
    }

}
