﻿using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class LaserRifle : Pickup
{
	public AudioClip FireSound;
    public AudioClip FireEmptySound;

    public GameObject MuzzleFlashPrefab;
    public GameObject BeamPrefab;
    public GameObject ImpactPrefab;

    public override string GetDescription()
    {
        return "Fires a narrow but high-powered laser beam which can cut a watermelon clean in half. Protective goggles are recommended during use.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo <= 0)
        {
            if(FireEmptySound != null)
                Helper.PlaySoundEffect(FireEmptySound);
        }
        else
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var beamDirection = rotation * Vector2.up;

            Helper.PlaySoundEffect(FireSound);

            // disable player's colliders while raycasting
            Player.GetComponent<Collider2D>().enabled = false;
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform).FirstOrDefault();
            bool isShieldEnabled = false;
            if (shield != null)
            {
                isShieldEnabled = shield.GetComponent<Collider2D>().enabled;
                shield.GetComponent<Collider2D>().enabled = false;
            }

            // set up collision layers - 1 is default
            var layerMask = 1 | LayerMask.GetMask("Shields");
            var hit = Physics2D.Raycast(origin, beamDirection, Mathf.Infinity, layerMask);

            if(hit.collider == null)
            {
                //Debug.Log("Laser didnt hit anything!?");
                hit.point = origin + (beamDirection * 100); // create a hit point so we can still draw a laser beam
                hit.normal = -beamDirection;
            }
            else
            {
                // do damage
                var dam = hit.collider.GetComponent<IDamageable>();
                if(dam != null)
                {
                    var amt = 20 * (1 - dam.GetResistances().Heat);
                    dam.Damage(20, gameObject);
                }
            }

            // re-enable colliders
            Player.GetComponent<Collider2D>().enabled = true;
            if (isShieldEnabled)
                shield.enabled = true;

            // this is how long the laser beam is visble for
            var laserLifetime = 0.1f;

            // muzzle flash
            var flash = Instantiate(MuzzleFlashPrefab);
            flash.transform.position = origin;
            flash.transform.rotation = rotation;
            flash.transform.SetParent(transform);
            Destroy(flash, laserLifetime);

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

            // create impact sprite
            var impactAngle = Mathf.Atan2(-hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
            var impact = Instantiate(ImpactPrefab, hit.point, Quaternion.AngleAxis(impactAngle, Vector3.forward));

            // tween alpha of beam alpha
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", laserLifetime, "onupdate", (Action<object>)(val => 
            { 
                var valf = (float)val;
                var startColor = new Color(1,0.2f,0.2f,valf);
                var endColor = new Color(1,0,0,valf);
                line.SetColors(startColor,endColor);

                line.SetPosition(0, Player.GetAimingOrigin());
            }), 
            "oncomplete", (Action)(() => 
            { 
                Destroy(beam.gameObject); 
            })));
            Ammo--;
        }
    }
}
