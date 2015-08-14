using UnityEngine;
using System.Collections;

public class MachineGun : Pickup
{
    public GameObject BulletPrefab;
    public AudioClip FireSound;

    private float fireDelay = 0.1f;
    private bool isTriggerDown= false;
    private float lastFireTime = 0;
    private GameObject muzzleFlash;

	// Use this for initialization
    void Start()
    {
        muzzleFlash = transform.FindChild("MuzzleFlash").gameObject;
        HideMuzzleFlash();
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
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, Player.GetAimingOrigin(), rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse); 
            bullet.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;

            ShowMuzzleFlash();
            
            GameBrain.Instance.WaitAndThenCall(fireDelay / 1.5f, HideMuzzleFlash);
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        isTriggerDown = false;
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
