using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {

	public GameObject explosionPrefab;
    public float baseDamage = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    void OnTriggerEnter2D(Collider2D other)
	{
        Explode(transform.position, transform.rotation);
        DamageOther(other.gameObject);
        Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
        var contact = collision.contacts[0];
        var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Explode(contact.point, rot);
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
    }

    void DamageOther(GameObject other)
    {
        if(baseDamage > 0)
        {
            var damageable = other.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.Damage(baseDamage, gameObject);
            }
        }
    }
}
