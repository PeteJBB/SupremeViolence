using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class FillBar: MonoBehaviour 
{
    private Transform bar;
    private Sprite barSprite;
    private float fullScale;

	// Use this for initialization
	void Start () 
    {
        bar = transform.Find("bar");
        barSprite = bar.GetComponent<SpriteRenderer>().sprite;
        fullScale = bar.localScale.x;
	}
	
    public void SetFill(float amt)
    {
        if(barSprite != null)
        {
            var scale = Mathf.Clamp(amt, 0, 1) * fullScale;
            bar.transform.localScale = new Vector3(scale, bar.localScale.y, bar.localScale.z);

            //var width = fullWidth - barSprite.bounds.size.x;
            //var left = -width / 2;
            //bar.transform.position = new Vector3(left, bar.position.y, bar.position.z);
        }
    }
}