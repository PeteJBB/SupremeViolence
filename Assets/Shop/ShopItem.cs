using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopItem: MonoBehaviour, IDeselectHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler
{
    public ShopWindow shopWindow;
    public Pickup pickup;
    public int PlayerIndex;

    private Text nameElem;
    private Text priceElem;
    private Text itemDesc;

    private PlayerState player;

    private bool isSelected = false;

    void Start()
    {
        nameElem = transform.Find("Name").GetComponent<Text>();
        priceElem = transform.Find("Price").GetComponent<Text>();

        player = GameState.Players[PlayerIndex];
        if(player.Pickups.Any(x => x == pickup))
        {
            priceElem.text = "Purchased";
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
            priceElem.text = "Purchased";
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isSelected)
        {
            UnhighlightItem();
        }
    }

    private void HighlightItem()
    {
        nameElem.color = Color.yellow;
        priceElem.color = Color.yellow;
        shopWindow.SetItemDesc(pickup.GetDescription());
    }

    private void UnhighlightItem()
    {
        nameElem.color = Color.white;
        priceElem.color = Color.white;
        shopWindow.SetItemDesc(string.Empty);
    }
}