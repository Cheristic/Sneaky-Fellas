using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float threshold;
    [SerializeField] float distanceLimiter;

    // Update is called once per frame
    void Update()
    {
        float finalAngle = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), player.transform.rotation.z);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 targetPos = finalDir.normalized * (Vector2.Distance(mousePos, player.position) / distanceLimiter);
        targetPos = Vector3.ClampMagnitude(targetPos, threshold) + player.position;
        targetPos.z = transform.position.z;

        transform.position = targetPos;
    }
}
