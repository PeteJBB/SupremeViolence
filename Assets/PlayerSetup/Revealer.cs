using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Revealer : MonoBehaviour 
{
    public bool StartImmediately = true;
    public UnityEvent OnRevealComplete;

    private float revealTime = 1;
    private Image image;
    private RectTransform rt;

	void Start () 
	{
        image = GetComponent<Image>();
        rt = GetComponent<RectTransform>();

        if (StartImmediately)
            StartReveal();
	}

    public void StartReveal()
    {
        Debug.Log("StartReveal");
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);
        
        image.enabled = true;

        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", revealTime, "easetype", iTween.EaseType.easeInCubic, "onupdate", (System.Action<object>)((obj) =>
        {
            var val = (float)obj;
            rt.anchorMax = new Vector2(1, val);
        }), 
        "oncomplete", (System.Action)(() =>
        {
            Finished();
        })));
    }

    private void Finished()
    {
        Debug.Log("Finished");
        image.enabled = false;
        if (OnRevealComplete != null)
            OnRevealComplete.Invoke();
    }
}
