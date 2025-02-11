using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Rigidbody2D rb;
    public Handgun_WC handgun;
    void Start()
    {
        rb.velocity = transform.right * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerHealthOld health;
        if (health = collider.GetComponent<PlayerHealthOld>())
        {
            health.GetHit(1, handgun.playerAttached.transform.GetChild(0).gameObject);
            Destroy(gameObject);
        }
        if (collider.gameObject.CompareTag("Obstacle")) { Destroy(gameObject); }
        
    }

}
