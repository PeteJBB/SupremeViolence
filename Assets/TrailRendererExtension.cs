using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TrailRendererExtension: MonoBehaviour 
{
    public string SortingLayerName;
    public int OrderInLayer;    

    private TrailRenderer trail;
    private bool isFadingOut = false;

	// Use this for initialization
	void Start () 
    {
        trail = GetComponent<TrailRenderer>();
        trail.sortingLayerName = SortingLayerName;
        trail.sortingOrder = OrderInLayer;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!isFadingOut && transform.parent == null)
        {
            // thats my queue to start fading out
            isFadingOut = true;
            var a = trail.material.GetColor("_TintColor").a;
            var time = trail.time / 2f;
            
            iTween.ValueTo(gameObject, iTween.Hash("from", a, "to", 0, "time", time, "onupdate", (Action<object>)((val) =>
            {
                var valf = (float)val;                
                trail.material.SetColor("_TintColor", new Color(1, 1, 1, valf));
            })));            
        }
	}
       
}