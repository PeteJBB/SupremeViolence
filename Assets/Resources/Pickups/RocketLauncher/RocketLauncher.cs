using UnityEngine;
using System.Collections;
using System.Linq;

public class RocketLauncher : Pickup
{
    public GameObject RocketPrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;

    private float lastFireTime;
    private float reloadTime = 2;
    private bool isReloading = false;    

    private SpriteRenderer laserSight;

    public override string GetDescription()
    {
        return "Rockets sure are fun aren't they? This is your high-explosive, point and shoot, no frills model favoured by terrorists and action movie stars alike.";
    }

    void Awake()
    {
        laserSight = transform.Find("lasersight").GetComponent<SpriteRenderer>();
        laserSight.enabled = false;

        base.OnSelectWeapon.AddListener(OnSelectWeapon_Handler);
        base.OnDeselectWeapon.AddListener(OnDeselectWeapon_Handler);
    }

    void Update()
    {
        if (Player.CurrentWeapon == this)
        {
            if (isReloading && Time.time - lastFireTime >= reloadTime)
            {
                isReloading = false;
                Player.RestoreAmmoBarSource();
                laserSight.enabled = true;
            }

            // laser sight angle
            var angle = Player.AimingAngle;
            laserSight.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            var offset = laserSight.transform.rotation * (Vector3.up * (laserSight.sprite.rect.height / laserSight.sprite.pixelsPerUnit) * (laserSight.sprite.pivot.y / laserSight.sprite.rect.height));
            laserSight.transform.position = Player.GetAimingOrigin().ToVector3() + offset;
        }
    }

    public void OnSelectWeapon_Handler()
    {
        if (!isReloading && Ammo > 0)
            laserSight.enabled = true;
    }

    public void OnDeselectWeapon_Handler()
    {
        laserSight.enabled = false;
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0 && !isReloading)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var rocket = (GameObject)GameObject.Instantiate(RocketPrefab, origin, rotation);

            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), rocket.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), rocket.GetComponent<Collider2D>());

            rocket.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            rocket.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);

            lastFireTime = Time.time;
            Ammo--;
            laserSight.enabled = false;

            if (Ammo > 0)
            {
                // show reloadtime in ammo bar
                isReloading = true;
                Player.SetAmmoBarSource(this, new Color(1, 0.5f, 0));
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(FireEmptySound, transform.position);
        }
    }

    public override float GetAmmoBarValue()
    {
        if (isReloading)
            return (Time.time - lastFireTime) / reloadTime;
        else 
            return base.GetAmmoBarValue();
    }
}
