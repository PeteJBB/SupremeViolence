using UnityEngine;
using System.Collections;
using System.Linq;

public class Flamer : Pickup
{
    public GameObject FlamePrefab;
    public AudioClip FireEmptySoundClip;
    public AudioClip FireJetSoundClip;
    public AudioClip FlameSoundClip;

    private float fireDelay = 0.04f;
    private bool isFiring = false;
    private float lastFireTime = 0;

    private AudioSource jetSound;
    private AudioSource flameSound;
    
	// Use this for initialization
	void Awake()
    {
        jetSound = gameObject.AddComponent<AudioSource>();
        jetSound.clip = FireJetSoundClip;
        jetSound.loop = true;
        jetSound.playOnAwake = false;

        flameSound = gameObject.AddComponent<AudioSource>();
        flameSound.clip = FlameSoundClip;
        flameSound.loop = true;
        flameSound.volume = 0;
        flameSound.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (isFiring && Time.time - lastFireTime > fireDelay)
        {
            if(Ammo > 0)
            {
                Fire();
                lastFireTime = Time.time;
            }
            else
            {
                StopFiring();
            }
        }
	}

    public override string GetDescription()
    {
        return "Whether you're warding off extraterrestrial invaders or just kick-starting a barbecue, the flamer guarantees delivery of charred and smouldering heaps on time, every time.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0)
        {
            StartFiring();
        }
        else
        {
            AudioSource.PlayClipAtPoint(FireEmptySoundClip, transform.position);
        }
    }

    float aimingError = 5;
    public void Fire()
    {
        var rotation = Quaternion.AngleAxis(Player.AimingAngle + Random.Range(-aimingError, aimingError), Vector3.forward);
        var flame = (GameObject)GameObject.Instantiate(FlamePrefab, Player.GetAimingOrigin(), rotation);
        flame.SetOwner(Player.gameObject);

        // ignore player
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), flame.GetComponent<Collider2D>());

        // ignore my own shield
        var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
        if(shield.Any())
            Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), flame.GetComponent<Collider2D>());

        var rb = flame.GetComponent<Rigidbody2D>();
        rb.AddRelativeForce(new Vector2(0, 0.4f), ForceMode2D.Impulse); 
        rb.AddTorque(Random.Range(-0.2f, 0.2f));

        Ammo--;
    }

    public override void OnFireUp(Vector3 origin)
    {
        StopFiring();
    }

    private void StartFiring()
    {
        isFiring = true;
        jetSound.Play();
        flameSound.Play();

        iTween.StopByName(gameObject, "flameFadeOut");
        iTween.AudioTo(gameObject, iTween.Hash("name", "flameFadeIn", "audiosource", flameSound, "volume", 1, "time", 1));
    }

    private void StopFiring()
    {
        isFiring = false;
        jetSound.Stop();

        iTween.StopByName(gameObject, "flameFadeIn");
        iTween.AudioTo(gameObject, iTween.Hash("name", "flameFadeOut", "audiosource", flameSound, "volume", 0, "time", 1, "oncomplete", "StopFlameSounds", "oncompletetarget", gameObject));
    }

    private void StopFlameSounds()
    {
        flameSound.Stop();
    }
}
