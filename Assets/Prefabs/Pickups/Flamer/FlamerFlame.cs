using UnityEngine;
using System.Collections;

public class FlamerFlame : MonoBehaviour
{
    private Animator anim;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D collision) 
    {
        var dam = collision.collider.GetComponent<Damageable>();
        if(dam != null)
        {
            dam.Damage(3, gameObject);
            //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
