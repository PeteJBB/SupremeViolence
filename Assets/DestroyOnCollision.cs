using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour
{
	public GameObject explosionPrefab;
    public float baseDamage = 0;
    public bool StepBackForExplosion = false;

    private Rigidbody2D rb;
    private Vector3 lastKnownVelocity;

    public bool CanBeReflected = false;
    public AudioClip CollisionSound;

	// Use this for initialization
	void Awake () 
	{
        rb = GetComponent<Rigidbody2D>();
	}

    void LateUpdate()
    {
        lastKnownVelocity = rb.velocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {            
            Explode(transform.position, transform.rotation);
            DamageOther(other.gameObject);
            Helper.DetachParticles(gameObject);
            Destroy(gameObject);
        }
    }

	void OnCollisionEnter2D(Collision2D collision) 
	{
        var contact = collision.contacts[0];
        var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);

        if (CanBeReflected)
        {
            var reflector = contact.collider.GetComponent<IReflector>();
            if (reflector != null && reflector.DoesReflectMe(gameObject))
            {
                // reflect
                var vect = contact.normal;// Vector2.Reflect(rb.velocity, contact.normal);
                rb.velocity = vect * lastKnownVelocity.magnitude;

                var owner = gameObject.GetOwner();
                if(owner != null)
                {
                    var ownerCollider = owner.GetComponent<Collider2D>();
                    var myCollider = GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(myCollider, ownerCollider, false);
                    gameObject.SetOwner(contact.collider.gameObject.GetOwner());
                }
                
                var angle = Mathf.Rad2Deg * Mathf.Atan2(-vect.x, vect.y);
                rb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                DamageOther(contact.collider.gameObject);

                return;
            }
        }

        // destroy 
        Vector2 collisionPoint;// = transform.position.ToVector2();
        if (StepBackForExplosion)
           collisionPoint = transform.position.ToVector2() - (lastKnownVelocity.ToVector2() * Time.deltaTime);
        else
            collisionPoint = contact.point;

        Explode(collisionPoint, rot);
        DamageOther(contact.collider.gameObject);

        Helper.DetachParticles(gameObject);
        Destroy(gameObject);

	}

    void Explode(Vector2 pos, Quaternion rot) 
    {
        if(explosionPrefab != null)
        {
            var exp = (GameObject)Instantiate(explosionPrefab, pos, rot);
            exp.SetOwner(gameObject.GetOwner());
        }

        if (CollisionSound != null)
        {
            Helper.PlaySoundEffect(CollisionSound);
        }
    }

    void DamageOther(GameObject other)
    {
        if(baseDamage > 0)
        {
            var damageable = other.GetComponent<IDamageable>();
            if(damageable != null)
            {
                var amt = baseDamage * (1 - damageable.GetResistances().Kinetic);
                damageable.Damage(amt, gameObject);
            }
        }
    }
}
