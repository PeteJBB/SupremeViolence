using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Dynamite : Pickup 
{
	public GameObject DynamiteStickPrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;

    public override string GetDescription()
    {
        return "Sticks of dynamite stolen from the set of a cowboy movie they're shooting next door.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var stick = (GameObject)GameObject.Instantiate(DynamiteStickPrefab);
            stick.transform.position = origin;
            
            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), stick.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), stick.GetComponent<Collider2D>());

            //stick.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 0.2f), ForceMode2D.Impulse);
            //stick.GetComponent<Rigidbody2D>().AddTorque(1);
            stick.SetOwner(Player.gameObject);
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
