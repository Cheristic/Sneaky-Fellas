using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float threshold;
    [SerializeField] float distance;

    // Update is called once per frame
    void Update()
    {
        float finalAngle = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), player.transform.rotation.z);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 targetPos = player.position + finalDir.normalized * (Vector3.Distance(mousePos, player.position) / distance);

        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }
}
