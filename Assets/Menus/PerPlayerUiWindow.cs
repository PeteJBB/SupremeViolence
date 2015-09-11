using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PerPlayerUiWindow: CustomMenuInputController 
{
    [HideInInspector]
    public RenderTexture renderTexture;

	// Use this for initialization
	void Awake () 
    {
	    // create render texture
        var rect = GetComponent<RectTransform>();
        renderTexture = new RenderTexture((int)rect.rect.width, (int)rect.rect.height, 0, RenderTextureFormat.Default);
        renderTexture.Create();

        var camera = transform.GetComponentInChildren<Camera>();
        camera.targetTexture = renderTexture;
	}
	
	public override void NavigateForwards(Canvas canvas)
    {
        Debug.Log("NavigateForwards to " + canvas.name);
        if(ActiveCanvas != null)
        {
            navStack.Push(ActiveCanvas);

            var rt = ActiveCanvas.GetComponent<RectTransform>();
            rt.offsetMin = new Vector2(0, -rt.rect.height);
            rt.offsetMax = new Vector2(0, rt.rect.height);       
            ActiveCanvas.gameObject.SetActive(false);
        }
        
        ActiveCanvas = canvas;

        if(ActiveCanvas != null)
        {
            ActiveCanvas.gameObject.SetActive(true);
            var rt = ActiveCanvas.GetComponent<RectTransform>();
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            
            //rt.rect.Set(0, 0, rt.rect.width, rt.rect.height); 

            SelectFirstVisibleItem();
        }
    }

    public override void GoBack()
    {
        if(navStack.Count > 0)
        {
            if(ActiveCanvas != null)
            {
                var rt = ActiveCanvas.GetComponent<RectTransform>();
                rt.offsetMin = new Vector2(0, -rt.rect.height);
                rt.offsetMax = new Vector2(0, rt.rect.height);       
                ActiveCanvas.gameObject.SetActive(false);
            }
            ActiveCanvas = navStack.Pop();

            if(ActiveCanvas != null)
            {
                ActiveCanvas.gameObject.SetActive(true);
                var rt = ActiveCanvas.GetComponent<RectTransform>();
                rt.offsetMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);

                SelectFirstVisibleItem();
            }
        }
    }
}