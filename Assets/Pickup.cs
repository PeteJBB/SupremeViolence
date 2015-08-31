using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Pickup : MonoBehaviour
{
    public string PickupName = "Pickup";
    public Sprite Icon;
    public PickupType PickupType;
    public AudioClip PickupSound;

    [HideInInspector]
    public UnityEvent OnPlayerPickup = new UnityEvent();

    [Tooltip("Shop price - items with price 0 will not show up in the shop")]
    public int Price = 0;


    [Tooltip("How much ammo this pickup starts with, -1 means unlimited ammo")]
    public int Ammo;

    [Tooltip("max ammo you can buy / carry for this pickup")]
    public int MaxAmmo;


    [HideInInspector]
    public PlayerControl Player = null;

    public virtual string GetPickupName()
    {
        return PickupName;
    }

    public virtual void CollectPickup(PlayerControl player)
    {
        // does player already own me?
        if (player.Pickups.Contains(this))
        {
            // just update my state
            this.Player = player;
            gameObject.SetOwner(player.gameObject);
            transform.SetParent(player.transform);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            // check if player already has one of these
            var duplicate = player.Pickups.Find(x => x.GetPickupName() == this.GetPickupName() && x != this);
            if (duplicate != null)
            {
                // already got one, take ammo etc and destroy
                if (GameBrain.Instance.State == PlayState.GameOn && PlayerHudCanvas.Instance != null)
                    PlayerHudCanvas.Instance.ShowPickupText("+Ammo", player.gameObject, player.PlayerIndex);

                var ammo = GetAmmoCount();
                if (ammo > 0)
                {
                    duplicate.AddAmmo(ammo);
                }
                Destroy(gameObject);
            }
            else
            {
                // pick it up
                this.Player = player;
                player.AddPickup(this);

                if (GameBrain.Instance.State == PlayState.GameOn && PlayerHudCanvas.Instance != null)
                    PlayerHudCanvas.Instance.ShowPickupText(this.GetPickupName(), player.gameObject, player.PlayerIndex);

                gameObject.SetOwner(player.gameObject);
                transform.SetParent(player.transform);
                transform.localPosition = Vector3.zero;

                OnPlayerPickup.Invoke();
            }

            // play pickup sound
            if (PickupSound != null)
                AudioSource.PlayClipAtPoint(PickupSound, transform.position);
        }
    }

    public virtual float GetAmmoBarValue()
    {
        return (float)Ammo / MaxAmmo;
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


    public virtual int GetAmmoCount()
    {
        return Ammo;
    }

    public virtual void AddAmmo(int amount)
    {
        if (Ammo > -1)
        {
            Ammo += amount;
            if (Ammo > MaxAmmo)
                Ammo = MaxAmmo;
        }
    }

    public virtual void OnSelectWeapon()
    {

    }

    public virtual void OnDeselectWeapon()
    {
        if (MaxAmmo > 0 && Ammo <= 0 && Player != null)
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
