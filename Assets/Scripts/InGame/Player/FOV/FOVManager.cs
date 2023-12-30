using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVManager : MonoBehaviour
{
    [SerializeField] private FOV_Object FOV_Cone;
    [SerializeField] private FOV_Object FOV_Circle;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = PlayerInterface.Main.playerObject.transform;
        MainPlayerController.updateFOV += UpdateFOV;
        FOV_Cone.transform.position = playerTransform.position;
        FOV_Circle.transform.position = playerTransform.position;
    }

    void UpdateFOV(Vector3 dir)
    {
        FOV_Cone.SetAimDirection(dir);
        FOV_Cone.SetOrigin(new Vector3(playerTransform.position.x, playerTransform.position.y, 0));
        FOV_Circle.SetAimDirection(dir);
        FOV_Circle.SetOrigin(new Vector3(playerTransform.position.x, playerTransform.position.y, 0));
    }
}
