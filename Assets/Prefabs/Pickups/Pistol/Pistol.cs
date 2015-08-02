using UnityEngine;
using System.Collections;

public class Pistol : Pickup
{
    public GameObject BulletPrefab;
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

    public virtual int GetAmmoCount()
    {
        return int.MaxValue;
    }

    public override void OnFireDown(Vector3 origin)
    {
        var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, origin, rotation);
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);

        AudioSource.PlayClipAtPoint(FireSound, transform.position);
    }
}
