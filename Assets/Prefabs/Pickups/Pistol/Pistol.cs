using UnityEngine;
using System.Collections;

public class Pistol : Pickup
{
    public GameObject BulletPrefab;

	// Use this for initialization
	void Start()
    {
        BaseStart();
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}


    public override bool CanPlayerPickup(PlayerControl player)
    {
        // look for other weapons and remove them
        for(var i=0; i < player.Pickups.Count; i++)
        {
            var p = player.Pickups[i];
            if(p.IsWeapon())
            {
                player.RemovePickup(p);
                Destroy(p.gameObject);
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
        var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, player.transform.position, rotation);
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 250));
    }
}
