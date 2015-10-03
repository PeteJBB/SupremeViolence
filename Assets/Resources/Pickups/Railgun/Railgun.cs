using UnityEngine;
using System;
using System.Collections;

public class Railgun : Pickup
{
    public GameObject BeamPrefab;
    public GameObject ImpactPrefab;

    private float chargingTime = 2f;
    private bool isCharging = false;
    private float chargeStartTime;

    public AudioClip FireSound;
    public AudioClip FireEmptySound;
    public AudioClip ChargingSoundClip;
    public AudioClip HummingSoundClip;

    private AudioSource chargingSound;
    private AudioSource humming;

    private ParticleSystem particles;
    private GameObject muzzleFlash;

    private SpriteRenderer laserSight;
    
	// Use this for initialization
    void Awake()
    {
        // sounds
        humming = gameObject.AddComponent<AudioSource>();
        humming.clip = HummingSoundClip;
        humming.loop = true;
        humming.playOnAwake = false;

        chargingSound = gameObject.AddComponent<AudioSource>();
        chargingSound.clip = ChargingSoundClip;
        chargingSound.playOnAwake = false;

        // particles
        particles = GetComponent<ParticleSystem>();
        particles.enableEmission = false;

        muzzleFlash = transform.FindChild("MuzzleFlash").gameObject;
        HideMuzzleFlash();

        laserSight = transform.Find("lasersight").GetComponent<SpriteRenderer>();
        laserSight.enabled = false;

        base.OnDeselectWeapon.AddListener(OnDeselectWeapon_Handler);
	}

    void Update()
    {
        if (Player != null && Player.CurrentWeapon == this)
        {
            var angle = Player.AimingAngle;
            laserSight.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            var offset = laserSight.transform.rotation * (Vector3.up * (laserSight.sprite.rect.height / laserSight.sprite.pixelsPerUnit) * (laserSight.sprite.pivot.y / laserSight.sprite.rect.height));
            laserSight.transform.position = Player.GetAimingOrigin().ToVector3() + offset;
        }
    }

    public void OnDeselectWeapon_Handler()
    {
        laserSight.enabled = false;
    }
	
    public override string GetDescription()
    {
        return "Hold down the trigger for a few seconds to charge this beastly contraption. Once charged release the trigger to hurl a hefty iron skewer at high velocity. Aim away from face.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0)
        {
            chargeStartTime = Time.time;
            isCharging = true;

            chargingSound.Play();
            chargingSound.volume = 1;
            chargingSound.pitch = 1;
            humming.volume = 1;
            humming.pitch = 1;

            humming.Play();
            chargingSound.Play();

            laserSight.enabled = false;

            iTween.StopByName(gameObject, "charge");
            iTween.ValueTo(gameObject, iTween.Hash("name", "charge", "from", 0f, "to", 1f, "time", chargingTime, "onupdate", (Action<object>)((o) =>
            {
                var val = (float)o;
                humming.volume = val;
            }),
            "oncomplete", (Action)(() =>
            {
                particles.enableEmission = true;
                laserSight.color = new Color(0, 1, 0, 0);
                laserSight.enabled = true;
                Helper.TweenSpriteAlpha(laserSight, 0, 1, 1f);
            })));

            Player.SetAmmoBarSource(this, new FillBarColorPoint[] { new FillBarColorPoint(new Color(0, 0.75f, 0), 0), new FillBarColorPoint(new Color(0, 1, 0), 1) });
        }
        else
        {
            Helper.PlaySoundEffect(FireEmptySound);
        }
    }

    public override void OnFireUp(Vector3 origin)
    {
        //chargeBar.Hide();
        Player.RestoreAmmoBarSource();
        laserSight.enabled = false;
        if(Ammo > 0)
        {
            humming.Stop();
            iTween.StopByName(gameObject, "charge");
            iTween.AudioTo(gameObject, iTween.Hash("name", "charge", "audiosource", chargingSound, "volume", 0, "time", 0.5f, "pitch", 0));

            if (Time.time - chargeStartTime > chargingTime)
            {
                var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
                var beamDirection = rotation * Vector2.up;
                
                Helper.PlaySoundEffect(FireSound);
                Player.GetComponent<Collider2D>().enabled = false;
                
                // set up collision layers - 1 is default
                var layerMask = 1 | LayerMask.GetMask("Shields");
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
                    var dam = hit.collider.GetComponent<IDamageable>();
                    if(dam != null)
                    {
                        var amt = 100 * (1 - dam.GetResistances().Kinetic);
                        dam.Damage(amt, gameObject);
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
                
                line.sortingLayerName = "Objects";
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
            isCharging = false;
        }
    }

    public override float GetLegStrengthMultiplier()
    {
        if (isCharging)
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

    public override float GetAmmoBarValue()
    {
        if (isCharging)
        {
            return (Time.time - chargeStartTime) / chargingTime;
        }

        return base.GetAmmoBarValue();
    }
}
