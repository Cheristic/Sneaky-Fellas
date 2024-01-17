using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : MonoBehaviour
{

    [Header("Lerping Properties")]
    private bool isLerpingPosition = false;
    private bool isLerpingRotation = false;
    private Vector3 realPosition;
    private Quaternion realRotation;
    private Vector3 lastRealPosition;
    private Quaternion lastRealRotation;
    private float timeStartedLerping;
    private float timeToLerp;

    private Transform playerObjectTransform;
    public void Init()
    {
        playerObjectTransform = GetComponent<NetworkPlayerInterface>().playerInterface.playerObject.transform;
        realPosition = playerObjectTransform.position;
        realRotation = playerObjectTransform.rotation;
    }
    public void ReceiveMovementMessage(SyncNetworkPlayersDataManager.PlayerMovementMessage msg)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition.Set(msg.pos.x, msg.pos.y, msg.pos.z);
        realRotation.Set(msg.rot.x, msg.rot.y, msg.rot.z, msg.rot.w);
        timeToLerp = msg.timeToLerp;

        if (realPosition != playerObjectTransform.position)
        {
            isLerpingPosition = true; // keep lerping
        }
        if (realRotation.eulerAngles != playerObjectTransform.rotation.eulerAngles)
        {
            isLerpingRotation = true; // keep lerping
        }

        timeStartedLerping = Time.time;
    }

    private void FixedUpdate()
    {
        // Update network player's position
        // PERFORM LERP
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            playerObjectTransform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            playerObjectTransform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }
}