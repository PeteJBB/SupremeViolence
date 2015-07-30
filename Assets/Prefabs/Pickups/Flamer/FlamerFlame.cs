using UnityEngine;
using System.Collections;

public class FlamerFlame : MonoBehaviour 
{
    float timeToFullSize = 1f;
    float tweenRatioTime = 1f;
    float fullSize = 2;
    float birthday;

    SpriteRenderer sprite;

	// Use this for initialization
	void Start () 
    {
        //transform.localScale = new Vector3(0,0,1);
        birthday = Time.time;
        //sprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var time = Time.time - birthday;
        if(time > timeToFullSize)
            Destroy(gameObject);
        else
        {
            // size
            var size = Mathf.Lerp(0, fullSize, time / timeToFullSize);

            // width/height ratio - flames get wider as they grow
            //var ratio = Mathf.Lerp(5, 1f, time / tweenRatioTime);
            //transform.localScale = new Vector3(size,size * ratio,1);

//            var r = Mathf.Lerp(Color.yellow.r, Color.red.r, time / timeToFullSize);
//            var g = Mathf.Lerp(Color.yellow.g, Color.red.g, time / timeToFullSize);
//            var b = Mathf.Lerp(Color.yellow.b, Color.red.b, time / timeToFullSize);
//            var a = Mathf.Lerp(1, 0, time / timeToFullSize);
//            sprite.color = new Vector4(r,g,b,a);
        }
	}
}
