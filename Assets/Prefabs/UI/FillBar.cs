using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class FillBar: MonoBehaviour 
{
    private SpriteRenderer frame;
    private SpriteRenderer bar;
    private float fullScale;

	// Use this for initialization
	void Awake () 
    {
        frame = GetComponent<SpriteRenderer>();
        bar = transform.Find("bar").GetComponent<SpriteRenderer>();

        fullScale = bar.transform.localScale.x;
	}
	
    public void SetFill(float amt)
    {
        var scale = Mathf.Clamp(amt, 0, 1) * fullScale;
        bar.transform.localScale = new Vector3(scale, bar.transform.localScale.y, bar.transform.localScale.z);
    }

    public void Hide(bool immediate = false)
    {
        if (immediate)
        {
            frame.color = new Color(0, 0, 0, 0);
            bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, 0);
            frame.enabled = false;
            bar.enabled = false;
        }
        else
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.15f, "easetype", iTween.EaseType.linear, "onupdate", (Action<object>)((val) =>
            {
                var a = (float)val;
                frame.color = new Color(0, 0, 0, a * 0.25f);
                bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, 0);
            }),
            "oncomplete", (Action)(() =>
            {
                frame.enabled = false;
                bar.enabled = false;
            })));
        }
    }

    public void Show()
    {
        frame.enabled = true;
        bar.enabled = true;
        frame.color = new Color(0, 0, 0, 0);
        bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, 0);

        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.15f, "easetype", iTween.EaseType.linear, "onupdate", (Action<object>)((val) =>
        {
            var a = (float)val;
            frame.color = new Color(0, 0, 0, a * 0.25f);

            var barColor = bar.color;
            barColor.a = a;
            bar.color = barColor;
        })));
    }
}