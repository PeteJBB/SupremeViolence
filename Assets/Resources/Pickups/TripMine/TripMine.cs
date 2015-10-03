using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TripMine : Pickup 
{
    public GameObject MinePrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;

    private SpriteRenderer lasersight_left;
    private SpriteRenderer lasersight_right;

    void Awake()
    {
        lasersight_left = transform.Find("lasersight_left").GetComponent<SpriteRenderer>();
        lasersight_left.enabled = false;

        lasersight_right = transform.Find("lasersight_right").GetComponent<SpriteRenderer>();
        lasersight_right.enabled = false;

        OnSelectWeapon.AddListener(OnSelectWeapon_handler);
        OnDeselectWeapon.AddListener(OnDeselectWeapon_handler);
    }

	public override string GetDescription()
    {
        return "Trip mine";
    }

    void Update()
    {
        if (Player != null && Player.CurrentWeapon == this)
        {
            lasersight_left.transform.rotation = Quaternion.AngleAxis(Player.AimingAngle + 45, Vector3.forward);
            var offset = lasersight_left.transform.rotation * (Vector3.up * (lasersight_left.sprite.rect.height / lasersight_left.sprite.pixelsPerUnit) * (lasersight_left.sprite.pivot.y / lasersight_left.sprite.rect.height));
            lasersight_left.transform.position = Player.GetAimingOrigin().ToVector3() + offset;

            lasersight_right.transform.rotation = Quaternion.AngleAxis(Player.AimingAngle - 45, Vector3.forward);
            offset = lasersight_right.transform.rotation * (Vector3.up * (lasersight_right.sprite.rect.height / lasersight_right.sprite.pixelsPerUnit) * (lasersight_right.sprite.pivot.y / lasersight_right.sprite.rect.height));
            lasersight_right.transform.position = Player.GetAimingOrigin().ToVector3() + offset;
        }
    }

    void OnSelectWeapon_handler()
    {
        lasersight_left.enabled = true;
        lasersight_right.enabled = true;
    }

    void OnDeselectWeapon_handler()
    {
        lasersight_left.enabled = false;
        lasersight_right.enabled = false;
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var mine = (GameObject)GameObject.Instantiate(MinePrefab);
            mine.transform.position = transform.position;
            mine.transform.rotation = rotation;

            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), mine.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), mine.GetComponent<Collider2D>());

            mine.SetOwner(Player.gameObject);
            if(FireSound != null)
                Helper.PlaySoundEffect(FireSound);
            Ammo--;
        }
        else
        {
            if(FireEmptySound != null)
                Helper.PlaySoundEffect(FireEmptySound);
        }
    }
}

