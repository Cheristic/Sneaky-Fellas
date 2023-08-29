using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking;

public class Handgun_WC : WeaponItemClass
{
    [Header("Handgun Attributes")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    public int ammoLeft;

    void Start()
    {
        itemName = "Handgun";
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

    public override void Use()
    {
        base.Use();
        if (ammoLeft == 0) return;

        ShootServerRpc();

        ammoLeft--;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc()
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<HandgunBullet>().handgun = this;
    }
}
