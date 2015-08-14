using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopItem: Selectable, IDeselectHandler, ISelectHandler, ISubmitHandler
{
    public ShopWindow shopWindow;
    public Pickup PickupPrefab;
    public int PlayerIndex;

    private Text nameElem;
    private Text ammoElem;
    private Text priceElem;

    private PlayerState player;

    private bool isSelected = false;

    void Awake()
    {
        nameElem = transform.Find("Name").GetComponent<Text>();
        ammoElem = transform.Find("Ammo").GetComponent<Text>();
        priceElem = transform.Find("Price").GetComponent<Text>();
    }

    void Start()
    {
        player = GameState.Players[PlayerIndex];
//        var p = player.Pickups.FirstOrDefault(x => x == PickupPrefab);
//        if(p != null)
//        {
//            ammoElem.text = string.Format("{0} / {1}", p.GetAmmoCount(), p.MaxAmmo);
//            if(p.Ammo == p.MaxAmmo)
//                priceElem.text = "Full";
//        }

        UpdateItemText();
    }

    private void UpdateItemText()
    {
        if(PickupPrefab != null && player != null)
        {
            nameElem.text = PickupPrefab.PickupName;
            priceElem.text = string.Format("{0:C0}", PickupPrefab.Price);

            // does player already own one of these
            var p = player.PickupStates.FirstOrDefault(x => x.Name == PickupPrefab.PickupName);
            if(p == null)
            {
                // not owned by player
                ammoElem.text = "-";
            }
            else
            {
                // owned already
                ammoElem.text = string.Format("{0} / {1}", p.Ammo, PickupPrefab.MaxAmmo);
                if(p.Ammo >= PickupPrefab.MaxAmmo)
                    priceElem.text = "Full";
            }
        }
    }

    public void OnSubmit (BaseEventData eventData)
    {
        // make purchase
        Debug.Log("Player " + (PlayerIndex + 1) + " bought " + nameElem.text);

        if(player.Cash >= PickupPrefab.Price)
        {
            // does player already own one of these
            var p = player.PickupStates.FirstOrDefault(x => x.Name == PickupPrefab.PickupName);
            if(p == null)
            {
                // add pickup to playerstate
                p = PickupState.FromPrefab(PickupPrefab);
                p.Ammo = 0; //<-- ammo gets set in a minute down
                player.PickupStates.Add(p);
            }

            if(p.Ammo < PickupPrefab.MaxAmmo)
            {
                // add ammo
                p.Ammo = Mathf.Clamp(p.Ammo + PickupPrefab.Ammo, 0, PickupPrefab.MaxAmmo);

                // pay!
                player.Cash -= PickupPrefab.Price;
            }

            UpdateItemText();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        HighlightItem();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        UnhighlightItem();
    }

    public void HighlightItem()
    {
        nameElem.color = Color.yellow;
        ammoElem.color = Color.yellow;
        priceElem.color = Color.yellow;
        shopWindow.SetItemDesc(PickupPrefab.GetDescription());
    }

    public void UnhighlightItem()
    {
        nameElem.color = Color.white;
        ammoElem.color = Color.white;
        priceElem.color = Color.white;
        shopWindow.SetItemDesc(string.Empty);
    }
}