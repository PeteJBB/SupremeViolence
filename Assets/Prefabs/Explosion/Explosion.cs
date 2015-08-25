using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
    public AudioClip explosionSound;

    private Animator anim;
    public float DamageRadius = 0.5f;
    public float MaxDamage = 100;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();

        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        var layerMask = 1 << LayerMask.NameToLayer("Default");
        var colliders = Physics2D.OverlapCircleAll(transform.position, DamageRadius, layerMask);
	
        transform.localScale = new Vector3(DamageRadius, DamageRadius, 1);

        foreach(var other in colliders)
        {
            var v = other.transform.position - transform.position;

            var dam = other.GetComponent<Damageable>();
            if(dam != null)
            {
                var ratio = v.magnitude / DamageRadius;
                var amt = Mathf.Lerp(MaxDamage, 0, ratio * ratio);
                var actualDamage = amt * (1 - dam.Resistance.Explosive);
                dam.Damage(actualDamage, gameObject);
            }

            var rb = other.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                var dragOrig = rb.drag;
                rb.drag = 0f;

                var maxForce = MaxDamage / 5;
                var amt = v.magnitude / DamageRadius;
                var actForce = Mathf.Lerp(maxForce, 0, amt * amt);
                rb.AddForce(v * actForce, ForceMode2D.Impulse);

                //var timeInAir = Mathf.Lerp(1, 0, v.magnitude / myCollider.radius);
                Helper.Instance.WaitAndThenCall(0.03f, () => 
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
