using UnityEngine;
using System.Collections;

public class FlamerFlame : MonoBehaviour 
{
    float timeToFullSize = 1f;
    float birthday;

    SpriteRenderer sprite;

	// Use this for initialization
	void Start () 
    {
        birthday = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        var time = Time.time - birthday;
        if(time > timeToFullSize)
            Destroy(gameObject);


	}

    void OnCollisionEnter2D(Collision2D collision) 
    {
        var dam = collision.collider.GetComponent<Damageable>();
        dam.Damage(3);
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
    }
}
