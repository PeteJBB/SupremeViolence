using UnityEngine;
using System.Collections;

public class Flamer : Pickup
{
    public GameObject FlamePrefab;
    private float fireDelay = 0.03f;
    private bool isTriggerDown= false;
    private float lastFireTime = 0;
    private PlayerControl player;

    public AudioClip FireSound;

	// Use this for initialization
	void Start()
    {
        BaseStart();
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (isTriggerDown && Time.time - lastFireTime > fireDelay)
        {
            Fire();
            lastFireTime = Time.time;
        }
	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override float GetMass()
    {
        return 0.1f;
    }

    private int fullAmmo = 1000;
    private int ammo = 1000;
    public override int GetAmmoCount()
    {
        return ammo;
    }

    public override void AddAmmo(int amount)
    {
        ammo += amount;
        if(ammo > fullAmmo)
            ammo = fullAmmo;
    }

    public override void OnFireDown(PlayerControl player)
    {
        isTriggerDown = true;
        this.player = player;
    }

    float aimingError = 5;
    public void Fire()
    {
        if(ammo > 0)
        {

            var rotation = Quaternion.AngleAxis(player.AimingAngle + Random.Range(-aimingError, aimingError), Vector3.forward);
            var flame = (GameObject)GameObject.Instantiate(FlamePrefab, player.transform.position, rotation);
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), flame.GetComponent<Collider2D>());
            var rb = flame.GetComponent<Rigidbody2D>();
            rb.AddRelativeForce(new Vector2(0, 0.3f), ForceMode2D.Impulse); 
            rb.AddTorque(Random.Range(-0.1f, 0.1f));
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            ammo--;
        }
    }

    public override void OnFireUp(PlayerControl player)
    {
        isTriggerDown = false;
    }

    public override void OnDeselectWeapon()
    {
        if(ammo <= 0)
        {
            // drop weapon
            player.RemovePickup(this);
        }
    }
}
