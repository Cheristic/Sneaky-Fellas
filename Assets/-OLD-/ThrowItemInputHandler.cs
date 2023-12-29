using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowItemInputHandler : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float threshold;
    [SerializeField] float offset;
    [System.NonSerialized] public float dropHoldTime;
    [SerializeField] LayerMask obstacleLayer;

    private bool isThrowing = false;

    private void Update()
    {
        if (isThrowing) PlaceThrowCircle();

    }
    public void HandleStartThrowInput(InputAction.CallbackContext obj)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        isThrowing = true;
    }

    private void PlaceThrowCircle()
    {
        float finalAngle = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        var finalDir = new Vector3(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle), player.transform.rotation.z);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = finalDir.normalized * Vector2.Distance(mousePos, player.position);
        targetPos = ClampMagnitude(targetPos, threshold, offset);
        targetPos.z = 0;

        RaycastHit2D rayHit = Physics2D.Raycast(player.position, finalDir, targetPos.magnitude, obstacleLayer);
        if (rayHit.collider == null)
        {
            //No hit
            transform.position = player.position + targetPos;
        }
        else
        {
            //Hit
            transform.position = new Vector3(rayHit.point.x, rayHit.point.y, player.position.z);
        }

        
    }

    public void HandleThrowInput(InputAction.CallbackContext obj)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        isThrowing = false;
    }

    private static Vector3 ClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        else if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
}
