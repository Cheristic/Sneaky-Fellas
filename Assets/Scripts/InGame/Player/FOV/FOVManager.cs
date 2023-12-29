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
        MainPlayerController.updateFOVDirection += UpdateFOVDirection;
    }

    void UpdateFOVDirection(Vector3 dir)
    {
        FOV_Cone.SetAimDirection(dir);
        FOV_Circle.SetAimDirection(dir);
    }
}
