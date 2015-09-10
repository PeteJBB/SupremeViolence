using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopItem: CustomSelectable, IDeselectHandler, ISelectHandler, ISubmitHandler
{
    public ShopWindow shopWindow;
    public Pickup PickupPrefab;
    public int PlayerIndex;

    private Text nameElem;
    private Text ammoElem;
    private Text priceElem;

    private PlayerState player;

    void Awake()
    {
        //background = transform.Find("background").GetComponent<Image>();
        nameElem = transform.Find("grid/Name").GetComponent<Text>();
        ammoElem = transform.Find("grid/Ammo").GetComponent<Text>();
        priceElem = transform.Find("grid/Price").GetComponent<Text>();
    }

    void Start()
    {
        player = GameState.Players[PlayerIndex];
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

    public override void OnSelect(BaseEventData eventData)
    {
        HighlightItem();        
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        UnhighlightItem();
        base.OnDeselect(eventData);
    }


    public void HighlightItem()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.1f, "onupdate", (System.Action<object>)((obj) =>
        {
            var val = (float)obj;
            Color col;
            col.r = Mathf.Lerp(colors.normalColor.r, colors.highlightedColor.r, val);
            col.g = Mathf.Lerp(colors.normalColor.g, colors.highlightedColor.g, val);
            col.b = Mathf.Lerp(colors.normalColor.b, colors.highlightedColor.b, val);
            col.a = Mathf.Lerp(colors.normalColor.a, colors.highlightedColor.a, val);
            
            nameElem.color = col;
            ammoElem.color = col;
            priceElem.color = col;

        })));

        shopWindow.SetItemDesc(PickupPrefab.GetDescription());
    }

    public void UnhighlightItem()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.1f, "onupdate", (System.Action<object>)((obj) =>
        {
            var val = (float)obj;
            Color col;
            col.r = Mathf.Lerp(colors.highlightedColor.r, colors.normalColor.r, val);
            col.g = Mathf.Lerp(colors.highlightedColor.g, colors.normalColor.g, val);
            col.b = Mathf.Lerp(colors.highlightedColor.b, colors.normalColor.b, val);
            col.a = Mathf.Lerp(colors.highlightedColor.a, colors.normalColor.a, val);
            
            nameElem.color = col;
            ammoElem.color = col;
            priceElem.color = col;

        })));
    }
}