using UnityEngine;
using System.Collections;

public class LaserRifle : Pickup
{
	public AudioClip FireSound;
    public AudioClip FireEmptySound;
    public LineRenderer line;
    public GameObject LightPrefab;
    public GameObject ImpactPrefab;

	// Use this for initialization
	void Start()
    {
        BaseStart();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        Ammo = MaxAmmo = 30;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if(Player != null && line.enabled)
        {
            line.SetPosition(0, Player.GetAimingOrigin());
        }
	}

    public override bool IsWeapon()
    {
        return true;
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo <= 0)
        {
            if(FireEmptySound != null)
                AudioSource.PlayClipAtPoint(FireEmptySound, transform.position);
        }
        else
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var beamDirection = rotation * Vector2.up;

            AudioSource.PlayClipAtPoint(FireSound, origin);
            Player.GetComponent<Collider2D>().enabled = false;

            // collide with the default layer only
            var layerMask = 1 << LayerMask.NameToLayer("Default");
            var hit = Physics2D.Raycast(origin, beamDirection, Mathf.Infinity, layerMask);

            if(hit.collider == null)
            {
                Debug.LogError("Laser didnt hit anything!?");
                hit.point = origin + (beamDirection * 100); // create a hit point so we can still draw a laser beam
                hit.normal = -beamDirection;
            }
            else
            {
                // do damage
                var dam = hit.collider.GetComponent<Damageable>();
                if(dam != null)
                {
                    dam.Damage(20, gameObject);
                }
            }
            line.enabled = true;
            line.SetPosition(0, origin);
            line.SetPosition(1, hit.point);

            line.SetColors(Color.red, Color.red);
            Player.GetComponent<Collider2D>().enabled = true;

            // this is how long the laser beam is visble for
            var laserLifetime = 0.1f;

            // create lights
            var d = 0f;
            var lightPre = LightPrefab.GetComponent<Light>();
            var step = lightPre.intensity * lightPre.range / 3;
            while(d < hit.distance)
            {
                var pos = transform.position + (beamDirection * d);
                var light = Instantiate(LightPrefab, pos, Quaternion.identity);
                d += step;

                Destroy(light, laserLifetime);
            }

            // create impact sprite
            var impactAngle = Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
            var impact = Instantiate(ImpactPrefab, hit.point, Quaternion.AngleAxis(impactAngle, Vector3.forward));

            // tween alpha
            iTween.StopByName(gameObject, "laser");
            iTween.ValueTo(gameObject, iTween.Hash("name", "laser", "from", 1, "to", 0, "time", laserLifetime, "onupdate", "TweenLaser", "oncomplete", "TweenLaserComplete"));

            Ammo--;
        }
    }

    void TweenLaser(float alpha)
    {
        var c = Color.red;
        c.a = alpha;
        line.SetColors(c, c);
    }

    void TweenLaserComplete()
    {
        line.enabled = false;
    }
}
