using UnityEngine;
using System.Collections;

public class Pistol : Pickup
{
    public GameObject BulletPrefab;
	public AudioClip FireSound;

    private GameObject muzzleFlash;

    public override string GetPickupName()
    {
        return "Pistol";
    }

	// Use this for initialization
	void Start()
    {
        BaseStart();
        Ammo = int.MaxValue;

        muzzleFlash = transform.FindChild("MuzzleFlash").gameObject;
        HideMuzzleFlash();
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override void OnFireDown(Vector3 origin)
    {
        var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, origin, rotation);
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);
        bullet.SetOwner(Player.gameObject);
        AudioSource.PlayClipAtPoint(FireSound, transform.position);

        ShowMuzzleFlash();

        GameBrain.Instance.WaitAndThenCall(0.1f, HideMuzzleFlash);
    }

    private void ShowMuzzleFlash()
    {
        muzzleFlash.GetComponent<Light>().enabled = true;
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = true;
        muzzleFlash.transform.position = Player.GetAimingOrigin().ToVector3();
    }
    
    private void HideMuzzleFlash()
    {
        muzzleFlash.GetComponent<Light>().enabled = false;
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = false;
    }
}
