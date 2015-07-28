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
        var arena = Transform.FindObjectOfType<Arena>();
        var emptySpots = arena.GetEmptyGridSpots();

        // choose a random spot
        var spot = emptySpots[Random.Range(0,emptySpots.Count)];
        transform.position = arena.GridToWorldPosition(spot);

        isDead = false;
        health = startingHealth;
        gameObject.SetActive(true);
    }

    public void Damage(float amount)
    {
        Debug.Log(gameObject.name + " damageable hit " + amount);
        health -= amount;
    }
}
