using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{

    public PlayerControl Player;
    public AudioClip PickupSound;

    private Transform background;
    private Transform icon;
    private bool isPickedUp = false;

    public int Ammo;
    public int MaxAmmo;

    void Start()
    {
        BaseStart();
    }

    public void BaseStart()
    {
        background = transform.FindChild("Background");
        icon = transform.FindChild("Icon");
    }

    public virtual string GetPickupName()
    {
        return "Default Pickup";
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
        // check if player already has one of these
        var duplicate = player.Pickups.Find(x => x.GetPickupName() == this.GetPickupName());
        if(duplicate != null)
        {
            // already got one, take ammo etc and destroy
            OnPickupDuplicate(player, duplicate);
            Destroy(gameObject);
        }
        else
        {
            // pick it up
            OnPlayerPickup(player);
            player.AddPickup(this);
            this.Player = player;
            gameObject.SetOwner(player.gameObject);

            if(icon != null)
                icon.gameObject.SetActive(false);
            if(background != null)
                background.gameObject.SetActive(false);
            
            isPickedUp = true;
            
            this.GetComponent<CircleCollider2D>().enabled = false;
            
            transform.parent = player.transform;
            transform.localPosition = Vector3.zero;
        }

        // free up grid point in arena
        Arena.Instance.RemoveGridObject(gameObject);

        // play pickup sound
        if(PickupSound != null)
            AudioSource.PlayClipAtPoint(PickupSound, transform.position);
    }

    public virtual bool IsWeapon()
    {
        return false;
    }

    public virtual float GetLegStrengthMultiplier()
    {
        return 1;
    }

    public virtual float GetMass()
    {
        return 0;
    }

    public virtual int GetPrice()
    {
        return 200;
    }

    public virtual string GetDescription()
    {
        return GetPickupName();
    }

    public virtual void OnPlayerPickup(PlayerControl player)
    {
        if(GameBrain.Instance.State == GameState.GameOn)
            PlayerHudCanvas.Instance.ShowPickupText(this.GetPickupName(), player.gameObject, player.PlayerIndex);
    }

    public virtual void OnPickupDuplicate(PlayerControl player, Pickup duplicate)
    {
        if(GameBrain.Instance.State == GameState.GameOn)
            PlayerHudCanvas.Instance.ShowPickupText("+Ammo", player.gameObject, player.PlayerIndex);

        var ammo = GetAmmoCount();
        if(ammo > 0)
        {
            duplicate.AddAmmo(ammo);
        }
    }

    public virtual int GetAmmoCount()
    {
        return Ammo;
    }

    public virtual void AddAmmo(int amount)
    {
        if(Ammo < int.MaxValue)
        {
            Ammo += amount;
            if(Ammo > MaxAmmo)
                Ammo = MaxAmmo;
        }
    }

    public virtual void OnSelectWeapon()
    {
        
    }

    public virtual void OnDeselectWeapon()
    {
        if(GetAmmoCount() <= 0 && Player != null)
            Player.RemovePickup(this);
    }



    public virtual void OnFireDown(Vector3 origin)
    {

    }

    public virtual void OnFireUp(Vector3 origin)
    {
        
    }

    public override string ToString()
    {
        return this.GetPickupName();
    }

}
