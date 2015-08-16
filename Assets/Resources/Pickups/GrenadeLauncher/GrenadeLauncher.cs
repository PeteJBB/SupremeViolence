using UnityEngine;
using System.Collections;

public class GrenadeLauncher : Pickup
{
    public GameObject GrenadePrefab;
    public AudioClip FireSound;

    public override string GetDescription()
    {
        return "Launches explosive grenades with a 2 second fuse and which can bounce off walls. Available now in time for mothers' day.";
    }

    public override void OnFireDown(Vector3 origin)
    {
        if(Ammo > 0)
        {
            var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
            var grenade = (GameObject)GameObject.Instantiate(GrenadePrefab, origin, rotation);
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), grenade.GetComponent<Collider2D>());
            grenade.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            grenade.GetComponent<Rigidbody2D>().AddTorque(1);
            grenade.SetOwner(Player.gameObject);
            AudioSource.PlayClipAtPoint(FireSound, transform.position);
            Ammo--;
        }
    }
}
