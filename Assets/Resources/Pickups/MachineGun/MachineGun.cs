using UnityEngine;
using System.Collections;

public class MachineGun : Pickup
{
    public GameObject MuzzleFlashPrefab;
    public GameObject BulletPrefab;
    public AudioClip FireSound;

    private float fireDelay = 0.1f;
    private bool isTriggerDown= false;
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
        return "This here is your standard high rate of fire meat grinder. Hold down the trigger and watch the other guy turn to mush.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        isTriggerDown = true;
    }

    public void FireBullet()
    {
        if(Ammo > 0)
        {
            var origin = Player.GetAimingOrigin();

            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, Player.GetAimingOrigin(), rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse); 
            bullet.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
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
