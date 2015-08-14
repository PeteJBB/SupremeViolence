using UnityEngine;
using System.Collections;

public class Railgun : Pickup
{
    public GameObject BulletPrefab;
    public AudioClip Fire;

    private AudioSource[] aSources;
    private float chargeTime;
    private bool charging = false;
    private float chargingTime = 2.6f;
    private AudioSource chargingSound;
    private AudioSource humming;
    private ParticleSystem particles;
   
	// Use this for initialization
    void Start()
    {
        //Initialise sounds
        aSources = this.GetComponents<AudioSource>();
        humming = aSources [1];
        chargingSound = aSources [0];

        //Initialise particles
        particles = GetComponent<ParticleSystem>();
        particles.enableEmission = false;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (charging && Time.time - chargeTime > chargingTime)
        {
            particles.enableEmission = true;
        }
	}

    public override string GetDescription()
    {
        return "Hold down the trigger for a few seconds to charge this beastly contraption. Once charged release the trigger to hurl a hefty iron skewer at high velocity. Aim away from face.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            chargeTime = Time.time;
            charging = true;
           // aSources = this.GetComponents<AudioSource>();
            chargingSound = aSources [0];
            chargingSound.Play();
            chargingSound.volume = 1;
            chargingSound.pitch = 1;
            humming.volume = 1;
            humming.pitch = 1;

            humming.Play();
            //iTween.MoveTo(camera1.gameObject, iTween.Hash("x", guy1.transform.position.x, "y", guy1.transform.position.y, "time", panTime, "easetype", iTween.EaseType.easeOutExpo));
            iTween.AudioFrom(gameObject, iTween.Hash("name", "RailgunHum", "audiosource", humming, "volume", 0, "time", chargingTime));
            //updateSound = true;
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        if(Ammo > 0)
        {
            if(aSources != null)
            {
                humming.Stop();
                iTween.StopByName(gameObject, "RailgunHum");
                iTween.AudioTo(gameObject, iTween.Hash("audiosource", chargingSound, "volume", 0, "time", 0.5f, "pitch", 0));
            }
            if (Time.time - chargeTime > chargingTime)
            {
                var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
                var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, origin, rotation);
                Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
                bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 30f), ForceMode2D.Impulse);
                bullet.SetOwner(Player.gameObject);
                AudioSource.PlayClipAtPoint(Fire, transform.position);
                Ammo--;
            } 
            particles.enableEmission = false;
            charging = false;
        }
    }

    public override float GetLegStrengthMultiplier()
    {
        if (charging)
        {
            return 0.5f;
        } 
        else
        {
            return base.GetLegStrengthMultiplier();
        }
    }
}
