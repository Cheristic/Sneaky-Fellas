using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerToTrack;

    // Update is called once per frame
    void Update()
    {
        if (playerToTrack == null) return;
        transform.position = new Vector3(playerToTrack.position.x, playerToTrack.position.y, transform.position.z);
    }
}
