using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerCameraMovement : NetworkBehaviour
{
    public static PlayerCameraMovement Instance { get; private set; }

    [HideInInspector] public Transform targetToTrack;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        transform.position = new Vector3(targetToTrack.position.x, targetToTrack.position.y, transform.position.z);
    }

}
