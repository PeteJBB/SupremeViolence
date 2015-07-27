using UnityEngine;
using System.Collections;

public class MachineGun : Pickup
{
    public GameObject BulletPrefab;
    private float fireDelay = 0.3f;
    private bool isTriggerDown= false;
    private float lastFireTime = 0;
    private PlayerControl player;

	// Use this for initialization
	void Start()
    {
        BaseStart();
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (isTriggerDown)
        {
            Debug.Log("MG Update");
            if(Time.time - lastFireTime > fireDelay)
            {
                FireBullet();
                lastFireTime = Time.time;
            }
        }

	}

    public override bool CanPlayerPickup(PlayerControl player)
    {
        // look for other weapons and remove them
        for(var i=0; i < player.Pickups.Count; i++)
        {
            var p = player.Pickups[i];
            if(p.IsWeapon())
            {
                player.Pickups.Remove(p);
                i--;
            }
        }

        return true;
    }

    public override bool IsWeapon()
    {
        return true;
    }

    public override void OnFireDown(PlayerControl player)
    {
        Debug.Log("Trigger Down");
        isTriggerDown = true;
        this.player = player;
    }

    public void FireBullet()
    {
        Debug.Log("Fire!");
        var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, player.transform.position, rotation);
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 250)); 
    }

   public override void OnFireUp(PlayerControl player)
    {
        Debug.Log("Trigger Up");
        isTriggerDown = false;
    }
}
