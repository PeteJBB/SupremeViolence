using UnityEngine;
using System.Collections;

public class GrenadeLauncher : Pickup
{
    public GameObject GrenadePrefab;
    public AudioClip FireSound;

	// Use this for initialization
	void Start()
    {
        BaseStart();
        Ammo = MaxAmmo = 8;
	}
	
    public override bool IsWeapon()
    {
        return true;
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var grenade = (GameObject)GameObject.Instantiate(GrenadePrefab, origin, rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), grenade.GetComponent<Collider2D>());
            grenade.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            grenade.GetComponent<Rigidbody2D>().AddTorque(1);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;
        }
    }
}
