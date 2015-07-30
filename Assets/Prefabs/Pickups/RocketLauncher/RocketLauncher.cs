using UnityEngine;
using System.Collections;

public class RocketLauncher : Pickup
{
    public GameObject RocketPrefab;
    public AudioClip FireSound;

	// Use this for initialization
	void Start()
    {
        BaseStart();
	}
	
	// Update is called once per frame
	void Update() 
    {

	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override float GetMass()
    {
        return 0.1f;
    }

    private int ammo = 8;
    public override int GetAmmoCount()
    {
        return ammo;
    }

    public override void AddAmmo(int amount)
    {
        ammo += amount;
        if(ammo > 8)
            ammo = 8;
    }

    public override void OnFireDown(PlayerControl player)
    {
        if(ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
            var rocket = (GameObject)GameObject.Instantiate(RocketPrefab, player.transform.position, rotation);
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), rocket.GetComponent<Collider2D>());
            rocket.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            ammo--;
        }
    }

    public override void OnDeselectWeapon()
    {
        if(ammo <= 0)
        {
            // drop weapon
            Player.RemovePickup(this);
        }
    }
}
