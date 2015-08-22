using UnityEngine;
using System;
using System.Collections;

public class Railgun : Pickup
{
    public GameObject BeamPrefab;
    public AudioClip FireSound;
    public GameObject ImpactPrefab;

    private AudioSource[] aSources;
    private float chargeTime;
    private bool charging = false;
    private float chargingTime = 2f;//2.6f;
    private AudioSource chargingSound;
    private AudioSource humming;
    private ParticleSystem particles;

    private GameObject muzzleFlash;


	// Use this for initialization
    void Start()
    {
        // sounds
        aSources = this.GetComponents<AudioSource>();
        humming = aSources [1];
        chargingSound = aSources [0];

        // particles
        particles = GetComponent<ParticleSystem>();
        particles.enableEmission = false;

        muzzleFlash = transform.FindChild("MuzzleFlash").gameObject;
        HideMuzzleFlash();
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
//                var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
//                var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, origin, rotation);
//                Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
//                bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 30f), ForceMode2D.Impulse);
//                bullet.SetOwner(Player.gameObject);
//                AudioSource.PlayClipAtPoint(Fire, transform.position);
//                Ammo--;

                var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
                var beamDirection = rotation * Vector2.up;
                
                AudioSource.PlayClipAtPoint(FireSound, origin);
                Player.GetComponent<Collider2D>().enabled = false;
                
                // collide with the default layer only
                var layerMask = 1 << LayerMask.NameToLayer("Default");
                var hit = Physics2D.Raycast(origin, beamDirection, Mathf.Infinity, layerMask);
                
                if(hit.collider == null)
                {
                    Debug.LogError("Railgun didnt hit anything!?");
                    hit.point = origin + (beamDirection * 100); // create a hit point so we can still draw a beam
                    hit.normal = -beamDirection;
                }
                else
                {
                    // do damage
                    var dam = hit.collider.GetComponent<Damageable>();
                    if(dam != null)
                    {
                        dam.Damage(100, gameObject);
                    }
                }

                Player.GetComponent<Collider2D>().enabled = true;


                // this is how long the beam is visble for
                var lineLifetime = 2f;

                // create beam
                var beam = Instantiate(BeamPrefab);
                beam.transform.position = transform.position;
                beam.transform.rotation = rotation;

                var line = beam.GetComponent<LineRenderer>();
                line.enabled = true;
                line.SetPosition(0, origin);
                line.SetPosition(1, hit.point);
                line.sortingOrder = SpriteSorter.GetOrderByYPosition(Mathf.Max(origin.y, hit.point.y));
                
                // align beam particle emitter
                var beamParticles = beam.GetComponentInChildren<ParticleSystem>();
                var beamLength = Vector2.Distance(origin, hit.point);
                beamParticles.transform.localScale = new Vector3(beamLength / 2, 1, 1);
                beamParticles.transform.localPosition = new Vector3(0, beamLength / 2, 0);

                beamParticles.enableEmission = true;

                // muzzle flash
                muzzleFlash.transform.rotation = rotation;
                ShowMuzzleFlash();

                // create impact sprite
                var impactAngle = Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
                var impact = Instantiate(ImpactPrefab, hit.point, Quaternion.AngleAxis(impactAngle, Vector3.forward));
                
                // tween color to grey vapourishness and then fade out alpha
                iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", lineLifetime, "onupdate", (Action<object>)(val => 
                { 
                    var valf = (float)val;
                    var rb = Mathf.Lerp(0, 0.5f, valf / (lineLifetime / 3));
                    var g = Mathf.Lerp(1, 0.5f, valf / (lineLifetime / 3));
                    var a = 1 - valf;
                    var color = new Color(rb,g,rb,a);
                    line.SetColors(color,color); 
                }), 
                "oncomplete", (Action)(() => 
                { 
                    Helper.DetachParticles(beam);
                    Destroy(line.gameObject); 
                })));

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

    private void ShowMuzzleFlash()
    {
        muzzleFlash.GetComponent<Light>().enabled = true;
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = true;
        muzzleFlash.transform.position = Player.GetAimingOrigin().ToVector3();

        var anim = muzzleFlash.GetComponent<Animator>();
        anim.Play(null);
        var animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Helper.Instance.WaitAndThenCall(animLength, HideMuzzleFlash);
    }
    
    private void HideMuzzleFlash()
    {
        muzzleFlash.GetComponent<Light>().enabled = false;
        muzzleFlash.GetComponent<SpriteRenderer>().enabled = false;
    }
}
