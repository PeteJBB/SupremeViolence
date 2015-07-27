using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour 
{
    public float startingHealth = 100;
    public float health;
    public GameObject explosionPrefab;
    public bool RespawnOnDeath = false;
    public float RespawnDelaySeconds = 1;

    private bool isDead = false;

	// Use this for initialization
	void Start () 
    {
        health = startingHealth;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (health <= 0 && !isDead)
        {
            isDead = true;

            // explode
            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
            }

            if(RespawnOnDeath)
            {
                gameObject.SetActive(false);
                GameBrain.Instance.WaitAndThenCall(() => Respawn(), RespawnDelaySeconds);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}

    public void Respawn() 
    {
        isDead = false;
        health = startingHealth;
        transform.position = new Vector2(Random.Range(-4f,4f),Random.Range(-4f,4f));
        gameObject.SetActive(true);
    }

    public void Damage(float amount)
    {
        Debug.Log(gameObject.name + " damageable hit " + amount);
        health -= amount;
    }
}
