using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShopWindow: CustomMenuInputController 
{
    public GameObject ShopItemPrefab;

    private Transform stockPanel;
    private Text itemDesc;
    private Text yourCash;

    [HideInInspector]
    public RenderTexture renderTexture;

    private List<ShopItem> shopItems = new List<ShopItem>();
    private int itemsPerPage = 6;
    private int numPages;
    private int currentPage = 0;

    void Awake()
    {
        // create render texture
        var rect = GetComponent<RectTransform>();
        renderTexture = new RenderTexture((int)rect.rect.width, (int)rect.rect.height, 0, RenderTextureFormat.Default);
        renderTexture.Create();
        
        var camera = GetComponentInChildren<Camera>();
        camera.targetTexture = renderTexture;
    }

	// Use this for initialization
	void Start () 
    {
        stockPanel = transform.Find("StockPanel");
        itemDesc = transform.Find("ItemDesc").GetComponent<Text>();
        yourCash = transform.Find("YourCash").GetComponent<Text>();

        var orderedPickups = GameSettings.PickupPrefabs.Where(x => x.Price > 0).OrderBy(x => x.Price).ToList();
        for(var i=0; i<orderedPickups.Count; i++)
        {
            var p = orderedPickups[i];

            var item = Instantiate(ShopItemPrefab).GetComponent<ShopItem>();
            item.transform.SetParent(stockPanel);
            item.pickup = p;
            item.shopWindow = this;
            item.PlayerIndex = PlayerIndex;

            shopItems.Add(item);

        }

        numPages = (int)Mathf.Ceil((float)shopItems.Count / itemsPerPage);
        RenderPage(0);
        base.Start();
	}

    // Update is called once per frame
    void Update () 
    {
        base.Update();
        yourCash.text = string.Format("Your Cash: {0:C0}", GameState.Players[PlayerIndex].Cash);
    }

    public void RenderPage(int index)
    {
        index = Mathf.Clamp(index, 0, numPages - 1);

        // disable all items
        shopItems.ForEach(item => { item.gameObject.SetActive(false); });

        // deselect the current item (only if its a shop item and not next/prev page or exit)
        var selectedShopItem = CurrentSelectedObject != null ? CurrentSelectedObject.GetComponent<ShopItem>() : null;
        if(selectedShopItem != null)
        {
            // call unhighlight manually because we're about to deactivate the item and it wont recieve the OnDeselect message
            selectedShopItem.UnhighlightItem();
            SetSelectedGameObject(null);
        }

        // enable the items on this page
        var pageItems = shopItems.Skip(itemsPerPage * index).Take(itemsPerPage).ToList();
        foreach(var item in pageItems)
        {
            item.gameObject.SetActive(true);
        }

        // if a shop item was selected before, select the first one on the new page
        if(selectedShopItem != null)
            SetSelectedGameObject(pageItems.First().gameObject);

        currentPage = index;

        var pageNoText = (currentPage + 1) + " / " + numPages;
        transform.Find("PrevPage/PageNo").GetComponent<Text>().text = pageNoText;
        transform.Find("NextPage/PageNo").GetComponent<Text>().text = pageNoText;
    }

    public override void NextPage()
    {
        RenderPage(currentPage+1);
    }

    public override void PrevPage()
    {
        RenderPage(currentPage-1);
    }

    public void ExitStore()
    {
        Application.LoadLevelAsync("Deathmatch");
    }

    public void SetItemDesc(string val)
    {
        itemDesc.text = val;
    }
	
	
}