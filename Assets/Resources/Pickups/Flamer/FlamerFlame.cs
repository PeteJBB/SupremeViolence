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
        var damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null)
        {
            // check damage against fire resistance
            var amt = 3 * (1 - damageable.GetResistances().Heat);
            damageable.Damage(amt, gameObject);
            //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
