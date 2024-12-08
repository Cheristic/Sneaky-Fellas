using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class FOVManager : MonoBehaviour
{
    [SerializeField] private FOV_Object FOV_Cone;
    [SerializeField] private FOV_Object FOV_Circle;
    private Transform playerTransform;

    private void Awake()
    {
        MainPlayerController.mainPlayerCreated += OnMainPlayerCreated;
        MainPlayerHealth.mainPlayerDied += UnlinkFOV;
        LinkFOV();
        SyncGameData.TriggerNewRoundReady.AddListener(LinkFOV);
    }

    private void OnMainPlayerCreated()
    {
        playerTransform = PlayerInterface.Main.playerObject.GetComponent<Rigidbody2D>().transform;
    }

    public void LinkFOV()
    {
        MainPlayerController.updateFOV += UpdateFOV;
        FOV_Cone.gameObject.SetActive(true);
        FOV_Circle.gameObject.SetActive(true);
    }

    void UpdateFOV(Vector3 dir)
    {
        if (playerTransform.IsUnityNull()) return;
        FOV_Cone.SetAimDirection(dir);
        FOV_Cone.SetOrigin(new Vector3(playerTransform.position.x, playerTransform.position.y, 0));
        FOV_Circle.SetAimDirection(dir);
        FOV_Circle.SetOrigin(new Vector3(playerTransform.position.x, playerTransform.position.y, 0));
    }

    void UnlinkFOV()
    {
        MainPlayerController.updateFOV -= UpdateFOV;
        FOV_Cone.gameObject.SetActive(false);
        FOV_Circle.gameObject.SetActive(false);
    }
}
