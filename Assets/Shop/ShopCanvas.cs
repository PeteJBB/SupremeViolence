using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopCanvas: MonoBehaviour 
{
    public GameObject ShopWindowPrefab;
    public GameObject ShopImagePrefab;

	// Use this for initialization
	void Start () 
    {
        GameState.StartNewGame(); //<-- this should happen on the main menu, really

        // clean up dev scene
        Destroy(GameObject.Find("ShopWindow").gameObject);
        Destroy(GetComponentInChildren<RawImage>().gameObject);

        for(var i=0; i<GameState.Players.Count; i++)
        {
            // create a ShopWindow which actually contains the UI for the shop
            // this is rendered to a RenderTexture that can then be show on a raw image in this canvas
            // this is so that i can scale things the way i want without fucking about with the GUI scaling systems
            var window = Instantiate(ShopWindowPrefab).GetComponent<ShopWindow>();
            window.PlayerIndex = i;
            window.name = "ShopWindow" + i;
            window.GetComponent<CustomMenuInputController>().PlayerIndex = i;

            var offsetRect = window.GetComponent<RectTransform>().rect;
            window.transform.position = new Vector3(-offsetRect.width * 2 * (i + 1), -offsetRect.height * 2 * (i + 1), 0);

            var image = Instantiate(ShopImagePrefab).GetComponent<RawImage>();
            image.transform.SetParent(transform);
            image.texture = window.renderTexture;
            image.name = "ShopImage" + i;

            var rect = image.GetComponent<RectTransform>();

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