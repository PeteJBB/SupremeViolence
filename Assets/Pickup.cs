using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    public string Name;
    private Transform background;
    private Transform icon;

    public AudioClip PickupSound;

    void Start()
    {
        BaseStart();
    }

    public void BaseStart()
    {
        background = transform.FindChild("Background");
        icon = transform.FindChild("Icon");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            var player = other.GetComponent<PlayerControl>();
            CollectPickup(player);
        }
    }

    public virtual void CollectPickup(PlayerControl player)
    {
        if(CanPlayerPickup(player))
        {
            Debug.Log("Picked up "+this.Name);
            player.AddPickup(this);

            if(icon != null)
                icon.gameObject.SetActive(false);
            if(background != null)
                background.gameObject.SetActive(false);

            this.GetComponent<CircleCollider2D>().enabled = false;
            
            transform.parent = player.transform;

            // free up grid point in arena
            var arena = Transform.FindObjectOfType<Arena>();
            arena.RemoveGridObject(gameObject);

            if(PickupSound != null)
                AudioSource.PlayClipAtPoint(PickupSound, transform.position);
        }
    }

    public virtual bool IsWeapon()
    {
        return false;
    }

    public virtual float GetMoveMultiplier()
    {
        return 1;
    }

    public virtual float GetLegStrengthMultiplier()
    {
        return 1;
    }

    public virtual float GetMass()
    {
        return 0;
    }

    public virtual bool CanPlayerPickup(PlayerControl player)
    {
        return true;
    }

    public virtual void OnFireDown(PlayerControl player)
    {

    }

    public virtual void OnFireUp(PlayerControl player)
    {
        
    }

}
