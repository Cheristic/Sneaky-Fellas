using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour
{
    Transform player;
    [SerializeField] float threshold;
    [SerializeField] float distanceLimiter;

    private void Start()
    {
        // Tells the player camera to start following this target
        PlayerCameraMovement.Instance.targetToTrack = transform;
        PlayerCameraMovement.Instance.GetComponent<CinemachineVirtualCamera>().Follow = transform;
        player = PlayerInterface.Main.playerObject.transform;
    }
    void Update()
    {
        float finalAngle = player.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), player.rotation.z);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 targetPos = finalDir.normalized * (Vector2.Distance(mousePos, player.position) / distanceLimiter);
        targetPos = Vector3.ClampMagnitude(targetPos, threshold) + player.position;
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }
}
