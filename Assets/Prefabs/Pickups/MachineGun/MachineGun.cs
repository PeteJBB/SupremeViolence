using UnityEngine;
using System.Collections;

public class MachineGun : Pickup
{
    public GameObject BulletPrefab;
    private float fireDelay = 0.1f;
    private bool isTriggerDown= false;
    private float lastFireTime = 0;

    public AudioClip FireSound;

	// Use this for initialization
	void Start()
    {
        BaseStart();
        Ammo = MaxAmmo = 30;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (isTriggerDown && Time.time - lastFireTime > fireDelay)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override void OnFireDown(Vector3 origin)
    {
        isTriggerDown = true;
    }

    public void FireBullet()
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, Player.GetAimingOrigin(), rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse); 
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        isTriggerDown = false;
    }
}
