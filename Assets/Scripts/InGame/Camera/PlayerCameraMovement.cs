using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerCameraMovement : NetworkBehaviour
{
    public static PlayerCameraMovement Instance { get; private set; }

    [HideInInspector] public Transform targetToTrack;

    public override void OnNetworkSpawn()
    {
        Instance = this;
        GetComponent<PixelPerfectCamera>().runInEditMode = true;
    }

    void Update()
    {
        if (targetToTrack == null) return;
        transform.position = new Vector3(targetToTrack.position.x, targetToTrack.position.y, transform.position.z);
    }

}
