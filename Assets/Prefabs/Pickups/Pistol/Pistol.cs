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

    public override void OnFireDown(PlayerControl player)
    {
        var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, player.transform.position, rotation);
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);

        AudioSource.PlayClipAtPoint(FireSound, transform.position);
    }
}
