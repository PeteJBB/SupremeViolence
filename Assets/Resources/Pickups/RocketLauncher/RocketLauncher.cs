using UnityEngine;
using System.Collections;
using System.Linq;

public class RocketLauncher : Pickup
{
    public GameObject RocketPrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;
    
    public override string GetDescription()
    {
        return "Rockets sure are fun aren't they? This is your high-explosive, point and shoot, no frills model favoured by terrorists and action movie stars alike.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if (Ammo > 0)
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
            Ammo--;
        }
        else
        {
            AudioSource.PlayClipAtPoint(FireEmptySound, transform.position);
        }
    }

    public override void OnDeselectWeapon()
    {
        if(Ammo <= 0)
        {
            // drop weapon
            Player.RemovePickup(this);
        }
    }
}
