using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour 
{
    public float health = 100;
    public GameObject explosionPrefab;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (health <= 0)
        {
            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
	}

    public void Damage(float amount)
    {
        Debug.Log("Damageable hit for " + amount);
        health -= amount;
    }
}
