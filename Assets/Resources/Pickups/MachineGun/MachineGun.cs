﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class MachineGun : Pickup
{
    public GameObject MuzzleFlashPrefab;
    public GameObject BulletPrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;

    private float fireDelay = 0.1f;
    private bool isTriggerDown = false;
    private float lastFireTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (isTriggerDown && Time.time - lastFireTime > fireDelay)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
    }

    public override string GetDescription()
    {
        return "Affordable firepower ideal for home protection or taking on a fascist regime. Keep the change ya filthy animal.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0)
            isTriggerDown = true;
        else
            Helper.PlaySoundEffect(FireEmptySound);
    }

    public void FireBullet()
    {
        if (Ammo > 0)
        {
            var origin = Player.GetAimingOrigin();

            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, Player.GetAimingOrigin(), rotation);

            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 0.1f), ForceMode2D.Impulse);
            bullet.SetOwner(Player.gameObject);
            Helper.PlaySoundEffect(FireSound);
            Ammo--;

            // muzzle flash
            var flash = Instantiate(MuzzleFlashPrefab);
            flash.transform.position = origin;
            flash.transform.rotation = rotation;
            flash.transform.SetParent(transform);
            Destroy(flash, 0.05f);
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        isTriggerDown = false;
    }
}
