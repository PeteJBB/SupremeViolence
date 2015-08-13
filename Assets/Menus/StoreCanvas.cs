using UnityEngine;
using System.Collections;

public class StoreCanvas: MonoBehaviour 
{
    public GameObject ShopWindowPrefab;

	// Use this for initialization
	void Start () 
    {
        // cleanup dev scene
        foreach(var w in GetComponentsInChildren<ShopWindow>())
        {
            Destroy(w.gameObject);
        }

        GameState.StartNewGame();
        for(var i=0; i<GameState.Players.Count; i++)
        {
            var window = Instantiate(ShopWindowPrefab).GetComponent<ShopWindow>();
            window.PlayerIndex = i;
            window.transform.SetParent(transform);
            window.name = "PlayerHud" + i;
            window.GetComponent<CustomMenuInputController>().PlayerIndex = i;
            var rect = window.GetComponent<RectTransform>();

            if(GameState.Players.Count == 2)
            {
                switch(i)
                {
                    case 0:
                        rect.anchorMin = new Vector2(0,0);
                        rect.anchorMax = new Vector2(0.5f,1);
                        break;
                    case 1:
                        rect.anchorMin = new Vector2(0.5f,0);
                        rect.anchorMax = new Vector2(1,1);
                        break;
                }
            }
            if(GameState.Players.Count > 2)
            {
                switch(i)
                {
                    case 0:
                        rect.anchorMin = new Vector2(0,0.5f);
                        rect.anchorMax = new Vector2(0.5f,1);
                        break;
                    case 1:
                        rect.anchorMin = new Vector2(0.5f,0.5f);
                        rect.anchorMax = new Vector2(1,1);
                        break;
                    case 2:
                        rect.anchorMin = new Vector2(0,0);
                        rect.anchorMax = new Vector2(0.5f,0.5f);
                        break;
                    case 3:
                        rect.anchorMin = new Vector2(0.5f,0);
                        rect.anchorMax = new Vector2(1,0.5f);
                        break;
                }
            }
            
            rect.offsetMin = new Vector2(0,0);
            rect.offsetMax = new Vector2(0,0);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}