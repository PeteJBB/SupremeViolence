using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopItem: MonoBehaviour, IDeselectHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler
{
    public ShopWindow shopWindow;
    public Pickup pickup;

    private Text nameElem;
    private Text priceElem;
    private Text itemDesc;

    private bool isSelected = false;

    void Start()
    {
        nameElem = transform.Find("Name").GetComponent<Text>();
        priceElem = transform.Find("Price").GetComponent<Text>();
    }

    public void OnSubmit (BaseEventData eventData)
    {
        // make purchase
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        HighlightItem();
    }

    public void OnDeselect (BaseEventData eventData)
    {
        isSelected = false;
        UnhighlightItem();
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        HighlightItem();
    }

    public void OnPointerExit (PointerEventData eventData)
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