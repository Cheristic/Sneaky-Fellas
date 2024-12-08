using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerCameraMovement : NetworkBehaviour
{
    [HideInInspector] public Transform targetToTrack;

    void Update()
    {
        if (targetToTrack == null) return;
        transform.position = new Vector3(targetToTrack.position.x, targetToTrack.position.y, transform.position.z);
    }

}
