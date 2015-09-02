using UnityEngine;
using System.Collections;
using System.Linq;

public class PlasmaGun : Pickup
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
        return "A phased plasma rifle in the 40 watt range. Now stocking at a department store near you (U.S. residents only)";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0)
            isTriggerDown = true;
        else
            AudioSource.PlayClipAtPoint(FireEmptySound, transform.position);
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

            var rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddRelativeForce(new Vector2(0, 0.08f), ForceMode2D.Impulse);
            //rb.AddTorque(1);

            bullet.SetOwner(Player.gameObject);

            if(FireSound != null)
                AudioSource.PlayClipAtPoint(FireSound, transform.position);

            Ammo--;

            // muzzle flash
            if (MuzzleFlashPrefab != null)
            {
                var flash = Instantiate(MuzzleFlashPrefab);
                flash.transform.position = origin;
                flash.transform.rotation = rotation;
                flash.transform.SetParent(transform);
                Destroy(flash, 0.05f);
            }
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        isTriggerDown = false;
    }
}
