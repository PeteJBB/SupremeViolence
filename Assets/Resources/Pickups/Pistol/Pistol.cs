using UnityEngine;
using System.Collections;

public class Pistol : Pickup
{
    public GameObject MuzzleFlashPrefab;
    public GameObject BulletPrefab;
    public AudioClip FireSound;


    public override void OnFireDown(Vector3 origin)
    {
        var rotation = Quaternion.AngleAxis(Player.AimingAngle, Vector3.forward);
        var bullet = (GameObject)GameObject.Instantiate(BulletPrefab, origin, rotation);
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);
        bullet.SetOwner(Player.gameObject);
        AudioSource.PlayClipAtPoint(FireSound, transform.position);

        // muzzle flash
        var flash = Instantiate(MuzzleFlashPrefab);
        flash.transform.position = origin;
        flash.transform.rotation = rotation;
        flash.transform.SetParent(transform);
        Destroy(flash, 0.1f);
    }
}
