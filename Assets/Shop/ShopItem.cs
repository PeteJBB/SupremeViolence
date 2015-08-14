using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopItem: Selectable, IDeselectHandler, ISelectHandler, ISubmitHandler
{
    public ShopWindow shopWindow;
    public Pickup pickup;
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
        if(pickup != null)
        {
            nameElem.text = pickup.PickupName;
            priceElem.text = string.Format("{0:C0}", pickup.Price);
        }

        if(GameState.Players != null)
        {
            player = GameState.Players[PlayerIndex];
            var p = player.Pickups.FirstOrDefault(x => x == pickup);
            if(p != null)
            {
                ammoElem.text = string.Format("{0} / {1}", p.GetAmmoCount(), p.MaxAmmo);
                if(p.Ammo == p.MaxAmmo)
                    priceElem.text = "Full";
            }
        }
    }

    public void OnSubmit (BaseEventData eventData)
    {
        // make purchase
        Debug.Log("Player " + (PlayerIndex + 1) + " bought " + nameElem.text);

        if(player.Cash >= pickup.Price)
        {
            // does player already own one of these
            var p = player.Pickups.FirstOrDefault(x => x.PickupName == pickup.PickupName);
            if(p == null)
            {
                // add to list
                p = GameObject.Instantiate(pickup).GetComponent<Pickup>();
                p.Ammo = 0;
                player.Pickups.Add(p);

            }

            if(p.Ammo < p.MaxAmmo)
            {
                // add ammo
                p.Ammo += pickup.StartAmmo;

                // set labels
                ammoElem.text = string.Format("{0} / {1}", p.Ammo, p.MaxAmmo);
                if(pickup.Ammo >= pickup.MaxAmmo)
                    priceElem.text = "Full";

                player.Cash -= pickup.Price;
            }
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
        shopWindow.SetItemDesc(pickup.GetDescription());
    }

    public void UnhighlightItem()
    {
        nameElem.color = Color.white;
        ammoElem.color = Color.white;
        priceElem.color = Color.white;
        shopWindow.SetItemDesc(string.Empty);
    }
}