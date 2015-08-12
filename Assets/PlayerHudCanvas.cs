
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;

public class PlayerHudCanvas : MonoBehaviour 
{
    private GameObject MessageTextTemplate;
    private GameObject PickupTextTemplate;

    public GameObject PlayerHudPrefab;

    private static PlayerHudCanvas _instance;
    public static PlayerHudCanvas Instance
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<PlayerHudCanvas>();
            return _instance;
        }
    }

	// Use this for initialization
	void Start () 
    {
        MessageTextTemplate = transform.FindChild("MessageTextTemplate").gameObject;
        PickupTextTemplate = transform.FindChild("PickupTextTemplate").gameObject;

        // create player HUDs
        for(var i=0; i<GameSettings.NumberOfPlayers; i++)
        {
            var hud = Instantiate(PlayerHudPrefab).GetComponent<PlayerHud>();
            hud.PlayerIndex = i;
            hud.transform.SetParent(transform);
            hud.name = "PlayerHud" + i;
            var rect = hud.GetComponent<RectTransform>();

            if(GameSettings.NumberOfPlayers == 2)
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
            if(GameSettings.NumberOfPlayers > 2)
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

    public Text ShowMessage(string msg, float seconds)
    {
        var textObj = (GameObject)Instantiate(MessageTextTemplate);
        var text = textObj.GetComponent<Text>();
        text.text = msg;

        textObj.transform.parent = transform;
        var rect = textObj.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;

        if(seconds > 0)
        {
            GameBrain.Instance.WaitAndThenCall(seconds, () => 
            {
                Destroy(text.gameObject);
            });
        }
        return text;
    }

    public Text ShowPickupText(string msg, GameObject obj, int playerIndex)
    {
        var textObj = (GameObject)Instantiate(PickupTextTemplate);
        var text = textObj.GetComponent<Text>();
        text.text = msg;
        
        textObj.transform.SetParent(transform);
        var rect = textObj.GetComponent<RectTransform>();
        var point = WorldToCanvasPoint(obj.transform.position, playerIndex);
        rect.anchoredPosition = point;

        iTween.MoveTo(textObj, iTween.Hash("y", point.y + 38, "time", 1));
        iTween.ValueTo(textObj, iTween.Hash("from", 1, "to", 0, "delay", 1, "time", 0.3f, "onupdate", (Action<object>) (newVal =>  
        { 
            var c = text.color; 
            c.a = (float)newVal; 
            text.color = c; 
        })));
  
        Destroy(text.gameObject, 1.3f);

        return text;
    }

    public Vector2 WorldToCanvasPoint(Vector3 worldPos, int playerNumber)
    {
        // first you need the RectTransform component of your canvas
        var canvasRect = GetComponent<RectTransform>();
        
        //then you calculate the position of the UI element
        // 0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        var camera = Camera.allCameras.FirstOrDefault(o => o.name == "PlayerCamera" + playerNumber);
        if(camera == null)
            return Vector2.zero;

        var screenPoint = camera.WorldToScreenPoint(worldPos);
        return screenPoint;
    }
}
