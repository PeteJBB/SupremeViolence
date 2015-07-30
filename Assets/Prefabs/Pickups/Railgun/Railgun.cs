using UnityEngine;
using System.Collections;

public class Railgun : Pickup
{
    public GameObject BulletPrefab;
    private float chargeTime;
    private bool charging = false;
    private float chargingTime = 3f;
    public AudioClip Fire;
    private AudioSource aSource;

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
        chargeTime = Time.time;
        charging = true;
        var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
        aSource = this.GetComponent<AudioSource>();
        aSource.Play();

    }

    public override void OnFireUp(PlayerControl player)
    {
        if (Time.time - chargeTime > chargingTime)
        {
            var rotation = Quaternion.AngleAxis(player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, player.transform.position, rotation);
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 30f), ForceMode2D.Impulse);
        
            AudioSource.PlayClipAtPoint(Fire, transform.position);
            charging = false;
        } else
        {
            //Stop charging sound?
            if(aSource != null)
            {
             aSource.Stop();
            }
            charging = false;
        }

        charging = false;
    }

    public override float GetLegStrengthMultiplier()
    {
        if (charging)
        {
            return 0.5f;
        } else
        {
            return base.GetLegStrengthMultiplier();
        }
    }
}
