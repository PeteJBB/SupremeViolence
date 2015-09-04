using UnityEngine;
using System.Collections;
using System.Linq;

public class GrenadeLauncher : Pickup
{
    public GameObject GrenadePrefab;
    public AudioClip FireSound;
    public AudioClip FireEmptySound;

    public override string GetDescription()
    {
        return "Say hello to my little friend! This sucker launches explosive grenades with a 2 second fuse which can bounce off walls. Available now in time for mothers' day.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var grenade = (GameObject)GameObject.Instantiate(GrenadePrefab, origin, rotation);
            
            // ignore player
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), grenade.GetComponent<Collider2D>());

            // ignore my own shield
            var shield = Helper.GetComponentsInChildrenRecursive<Shield>(Player.transform);
            if (shield.Any())
                Physics2D.IgnoreCollision(shield.First().GetComponent<Collider2D>(), grenade.GetComponent<Collider2D>());

            grenade.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            grenade.GetComponent<Rigidbody2D>().AddTorque(1);
            grenade.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;
        }
        else
        {
            AudioSource.PlayClipAtPoint(FireEmptySound, transform.position);
        }
    }
}
