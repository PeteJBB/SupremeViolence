using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class Minigun : Pickup
{
    public GameObject MuzzleFlashPrefab;
    public GameObject BulletPrefab;

    public AudioClip SpinUpClip;
    public AudioClip SpinClip;
    public AudioClip SpinDownClip;
    public AudioClip FireClip;

    private AudioSource spinUpSound;
    private AudioSource spinSound;
    private AudioSource fireSound;
    private AudioSource spinDownSound;

    private float spinUpTime = 0.5f;
    private float fireDelay = 0.04f;
    private bool isTriggerDown = false;
    private float lastFireTime = 0;


    void Awake()
    {
        spinUpSound = gameObject.AddComponent<AudioSource>();
        spinUpSound.clip = SpinUpClip;
        spinUpSound.playOnAwake = false;

        spinSound = gameObject.AddComponent<AudioSource>();
        spinSound.clip = SpinClip;
        spinSound.playOnAwake = false;
        spinSound.loop = true;

        fireSound = gameObject.AddComponent<AudioSource>();
        fireSound.clip = FireClip;
        fireSound.playOnAwake = false;

        spinDownSound = gameObject.AddComponent<AudioSource>();
        spinDownSound.clip = SpinDownClip;
        spinDownSound.playOnAwake = false;
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
        return "This here is your ridiculous high rate of fire meat grinder. If it bleeds you can kill it with a minigun.";
    }

    public override float GetLegStrengthMultiplier()
    {
        return isTriggerDown
            ? 0.5f
            : 1;
    }

    public override void OnFireDown(Vector3 origin)
    {
        isTriggerDown = true;

        float timeToSpinUp;

        if (spinDownSound.isPlaying && spinDownSound.time < spinUpTime)
        {
            var ratio = spinDownSound.time / spinDownSound.clip.length;
            timeToSpinUp = spinUpTime * ratio;
            spinUpSound.time = spinUpSound.clip.length - (spinUpSound.clip.length * ratio);

            Debug.Log("timeToSpinUp " + timeToSpinUp);
        }
        else
        {
            timeToSpinUp = spinUpTime;
            spinUpSound.time = 0;
        }

        spinDownSound.Stop();
        spinUpSound.Play();
        lastFireTime = Time.time + timeToSpinUp - fireDelay;

        //iTween.AudioTo(gameObject, iTween.Hash("name", "spin", "audiosource", spinSound, "pitch", 1, "volume", 1, "time", spinUpTime)); 
    }

    public override void OnFireUp(Vector3 origin)
    {
        isTriggerDown = false;

        spinUpSound.Stop();
        spinSound.Stop();
        spinDownSound.Play();
    }

    public void FireBullet()
    {
        if (Ammo > 0)
        {
            var origin = Player.GetAimingOrigin();

            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, Player.GetAimingOrigin(), rotation);

            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());

            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);
            bullet.SetOwner(Player.gameObject);

            if (!spinSound.isPlaying)
                spinSound.Play();

            fireSound.time = 0;
            if(!fireSound.isPlaying)
                fireSound.Play();

            Ammo--;

            // muzzle flash
            var flash = Instantiate(MuzzleFlashPrefab);
            flash.transform.position = origin;
            flash.transform.rotation = rotation;
            flash.transform.SetParent(transform);
            Destroy(flash, 0.05f);
        }
    }
}
