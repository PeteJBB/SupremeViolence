using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class ShopWindow: MonoBehaviour 
{
    public GameObject ShopItemPrefab;
    public int PlayerIndex;

    private Transform stockPanel;
    private Text itemDesc;
    private Text cashValElem;

    private GridLayoutGroup gridLayout;

    public RenderTexture renderTexture;

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
        //itemDesc = transform.Find("ItemDesc").GetComponent<Text>();
        //cashValElem = transform.Find("CashVal").GetComponent<Text>();

        var orderedPickups = GameSettings.PickupPrefabs.OrderBy(x => x.GetPrice()).ToList();
        for(var i=0; i<orderedPickups.Count; i++)
        {
            var p = orderedPickups[i];
            var item = Instantiate(ShopItemPrefab).GetComponent<ShopItem>();
            item.transform.SetParent(stockPanel);
            item.transform.Find("Name").GetComponent<Text>().text = p.GetPickupName();
            item.transform.Find("Price").GetComponent<Text>().text = string.Format("{0:C0}", p.GetPrice());
            item.shopWindow = this;
            item.pickup = p;
            item.PlayerIndex = PlayerIndex;

//            var rect = item.GetComponent<RectTransform>();
//            var h = rect.rect.height;
//            rect.offsetMin = new Vector2(0, (-h * i) - h);
//            rect.offsetMax = new Vector2(0, -h * i);
        }
	}

    public void ExitStore()
    {
        Application.LoadLevelAsync("Deathmatch");
    }

    public void SetItemDesc(string val)
    {
        itemDesc.text = val;
    }
	
	// Update is called once per frame
	void Update () 
    {
        //cashValElem.text = string.Format("{0:C0}", GameState.Players[PlayerIndex].Cash);
	}
}