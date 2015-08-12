using UnityEngine;
using System.Collections;

public class RocketLauncher : Pickup
{
    public GameObject RocketPrefab;
    public AudioClip FireSound;

    public override string GetPickupName()
    {
        return "Rocket Launcher";
    }

	// Use this for initialization
	void Start()
    {
        BaseStart();
        Ammo = MaxAmmo = 5;
	}
	
	// Update is called once per frame
	void Update() 
    {

	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override int GetPrice()
    {
        return 900;
    }
    
    public override string GetDescription()
    {
        return "Rockets sure are fun aren't they? This is your high-explosive, point and shoot, no frills model favoured by terrorists, and action movie stars alike.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var rocket = (GameObject)GameObject.Instantiate(RocketPrefab, origin, rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), rocket.GetComponent<Collider2D>());
            rocket.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            rocket.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;
        }
    }

    public override void OnDeselectWeapon()
    {
        if(Ammo <= 0)
        {
            // drop weapon
            Player.RemovePickup(this);
        }
    }
}
