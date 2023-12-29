using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class KnifeWeaponParent : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float delay = 0.3f;
    private bool attackBlocked;

    [SerializeField] Transform circleOrigin;
    [SerializeField] float radius;


    void Start()
    {
        if (!IsOwner)
        {
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("BehindMask");
            return;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
    }

    public override void OnNetworkSpawn()
    {

    }

    public void PlayerAttack()
    {
        if (attackBlocked) return;
        animator.SetTrigger("Attack");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetectCollidersServerRpc()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            //if (collider.GetComponent<NetworkObject>().OwnerClientId == GetComponent<Knife_WC>().clientOwnerId) return;
            PlayerHealthOld health;
            if (health = collider.GetComponent<PlayerHealthOld>())
            {
                health.GetHit(1, GetComponent<ItemClass>().playerAttached.transform.GetChild(0).gameObject);
            }
        }
    }
}
