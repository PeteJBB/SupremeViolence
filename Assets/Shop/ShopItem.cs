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

    void Start()
    {
        nameElem = transform.Find("Name").GetComponent<Text>();
        ammoElem = transform.Find("Ammo").GetComponent<Text>();
        priceElem = transform.Find("Price").GetComponent<Text>();

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

        var price = pickup.GetPrice();
        if(player.Cash >= price)
        {
            player.Cash -= price;
            player.Pickups.Add(pickup);
            ammoElem.text = string.Format("{0} / {1}", pickup.GetAmmoCount(), pickup.MaxAmmo);
            priceElem.text = "Full";
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

    private void HighlightItem()
    {
        nameElem.color = Color.yellow;
        ammoElem.color = Color.yellow;
        priceElem.color = Color.yellow;
        shopWindow.SetItemDesc(pickup.GetDescription());
    }

    private void UnhighlightItem()
    {
        nameElem.color = Color.white;
        ammoElem.color = Color.white;
        priceElem.color = Color.white;
        shopWindow.SetItemDesc(string.Empty);
    }
}