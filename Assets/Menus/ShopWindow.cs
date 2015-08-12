using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopWindow: MonoBehaviour 
{
    public GameObject ShopItemPrefab;
    private Transform stockPanel;
    private Text itemDesc;

	// Use this for initialization
	void Start () 
    {
        stockPanel = transform.Find("StockPanel");
        itemDesc = transform.Find("ItemDesc").GetComponent<Text>();

        for(var i=0; i<GameSettings.PickupPrefabs.Length; i++)
        {
            var p = GameSettings.PickupPrefabs[i];
            var item = Instantiate(ShopItemPrefab).GetComponent<ShopItem>();
            item.transform.SetParent(stockPanel);
            item.transform.Find("Name").GetComponent<Text>().text = p.GetPickupName();
            item.transform.Find("Price").GetComponent<Text>().text = string.Format("{0:C0}", p.GetPrice());
            item.shopWindow = this;
            item.pickup = p;

            var rect = item.GetComponent<RectTransform>();
            var h = rect.rect.height;
            rect.offsetMin = new Vector2(0, (-h * i) - h);
            rect.offsetMax = new Vector2(0, -h * i);
        }
	}

    public void SetItemDesc(string val)
    {
        itemDesc.text = val;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}