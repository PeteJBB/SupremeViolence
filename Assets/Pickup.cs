using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    private Transform background;
    private Transform icon;

    public string PickupName = "Pickup";
    public int Price = 0; // items with price 0 will not show up in the shop

    public PickupType PickupType;

    [Tooltip("How much ammo this pickup starts with, -1 means unlimited ammo")]
    public int Ammo;// -1 means unlimited ammo

    [Tooltip("max ammo you can buy / carry for this pickup")]
    public int MaxAmmo;

    public AudioClip PickupSound;

    [HideInInspector]
    public PlayerControl Player;
    
    void Awake()
    {
        background = transform.FindChild("Background");
        icon = transform.FindChild("Icon");
    }

    public virtual string GetPickupName()
    {
        return PickupName;
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
        // does player already own me?
        if(player.Pickups.Contains(this))
        {
            // just update my state
            this.Player = player;
            gameObject.SetOwner(player.gameObject);
            transform.SetParent(player.transform);
            transform.localPosition = Vector3.zero;
            HidePickupIcons();
        }
        else
        {
            // check if player already has one of these
            var duplicate = player.Pickups.Find(x => x.GetPickupName() == this.GetPickupName() && x != this);
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
                transform.SetParent(player.transform);
                transform.localPosition = Vector3.zero;
                HidePickupIcons();
            }

            // free up grid point in arena
            Arena.Instance.RemoveGridObject(gameObject);

            // play pickup sound
            if(PickupSound != null)
                AudioSource.PlayClipAtPoint(PickupSound, transform.position);
        }
    }

    private void HidePickupIcons()
    {
        if(icon != null)
            icon.gameObject.SetActive(false);
        if(background != null)
            background.gameObject.SetActive(false);

        this.GetComponent<CircleCollider2D>().enabled = false;
    }

    public virtual float GetLegStrengthMultiplier()
    {
        return 1;
    }

    public virtual float GetMass()
    {
        return 0;
    }

    public virtual string GetDescription()
    {
        return "";
    }

    public virtual void OnPlayerPickup(PlayerControl player)
    {
        if(GameBrain.Instance.State == PlayState.GameOn)
            PlayerHudCanvas.Instance.ShowPickupText(this.GetPickupName(), player.gameObject, player.PlayerIndex);
    }

    public virtual void OnPickupDuplicate(PlayerControl player, Pickup duplicate)
    {
        if(GameBrain.Instance.State == PlayState.GameOn)
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
        if(Ammo > -1)
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

public enum PickupType
{
    Weapon,
    Equipment, 
    Passive
}
