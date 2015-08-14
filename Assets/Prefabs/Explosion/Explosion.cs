using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
    public AudioClip explosionSound;

    private Animator anim;
    private float DamageRadius = 0.5f;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();

        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        var colliders = Physics2D.OverlapCircleAll(transform.position, DamageRadius);
	
        foreach(var other in colliders)
        {
            var v = other.transform.position - transform.position;

            var dam = other.GetComponent<Damageable>();
            if(dam != null)
            {
                var maxDamage = 100;
                var actualDamage = Mathf.Lerp(maxDamage, 0, v.magnitude / DamageRadius);
                dam.Damage(actualDamage, gameObject);
            }

            var rb = other.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                var dragOrig = rb.drag;
                rb.drag = 0f;

                var maxForce = 100;
                var actForce = Mathf.Lerp(maxForce, 0, v.magnitude / DamageRadius);
                rb.AddForce(v * actForce, ForceMode2D.Impulse);

                //var timeInAir = Mathf.Lerp(1, 0, v.magnitude / myCollider.radius);
                Helper.Instance.WaitAndThenCall(0.1f, () => 
                {
                    if(rb != null)
                        rb.drag = dragOrig;
                });
            }
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if(state.normalizedTime >= 1)
        {
            // detach particles
            Helper.DetachParticles(gameObject);
            Destroy(gameObject);
        }
	}
}
