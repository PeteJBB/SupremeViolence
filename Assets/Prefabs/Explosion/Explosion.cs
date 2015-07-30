﻿using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
    public AudioClip explosionSound;

    private Animator anim;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();

        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        var myCollider = GetComponent<CircleCollider2D>();
        var v = other.transform.position - transform.position;

        var dam = other.GetComponent<Damageable>();
        if(dam != null)
        {
            var maxDamage = 100;
            var actualDamage = Mathf.Lerp(maxDamage, 0, v.magnitude / myCollider.radius);
            dam.Damage(actualDamage);
        }

        var rb = other.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            var dragOrig = rb.drag;
            rb.drag = 0f;

            var maxForce = 100;
            var actForce = Mathf.Lerp(maxForce, 0, v.magnitude / myCollider.radius);
            rb.AddForce(v * actForce, ForceMode2D.Impulse);

            //var timeInAir = Mathf.Lerp(1, 0, v.magnitude / myCollider.radius);
            GameBrain.Instance.WaitAndThenCall(0.1f, () => 
            {
                rb.drag = dragOrig;
            });
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
