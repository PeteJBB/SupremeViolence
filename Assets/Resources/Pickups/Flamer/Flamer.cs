using UnityEngine;
using System.Collections;

public class Flamer : Pickup
{
    public GameObject FlamePrefab;

    private float fireDelay = 0.04f;
    private bool isFiring = false;
    private float lastFireTime = 0;

    private AudioSource jetSound;
    private AudioSource flameSound;

	// Use this for initialization
	void Start()
    {
        jetSound = GetComponents<AudioSource>()[0];
        flameSound = GetComponents<AudioSource>()[1];
        flameSound.volume = 0;
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
        if(Ammo > 0)
        {
            StartFiring();
        }
    }

    float aimingError = 5;
    public void Fire()
    {
        var rotation = Quaternion.AngleAxis(Player.AimingAngle + Random.Range(-aimingError, aimingError), Vector3.forward);
        var flame = (GameObject)GameObject.Instantiate(FlamePrefab, Player.GetAimingOrigin(), rotation);
        flame.SetOwner(Player.gameObject);

        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), flame.GetComponent<Collider2D>());
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
