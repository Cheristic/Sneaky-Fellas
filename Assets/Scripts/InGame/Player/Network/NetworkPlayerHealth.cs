using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerHealth : PlayerHealth
{
    public override void Die()
    {
        PlayerInterface i = GetComponentInParent<PlayerInterface>();
        i.playerObject.layer = 13; // PlayerDead
        i.playerSprite.gameObject.layer = 13;

        i.playerObject.GetComponent<PolygonCollider2D>().enabled = false;
        isDead = true;
        i.playerSprite.OnDie();
    }
}
