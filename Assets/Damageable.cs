using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour 
{
    public float startingHealth = 100;
    public float health;
    public GameObject explosionPrefab;
    public bool RespawnOnDeath = false;
    public float RespawnDelaySeconds = 1;

    public AudioClip DeathSound;
    public int PointsValue = 0;

    public bool IsDead = false;

	// Use this for initialization
	void Start () 
    {
        health = startingHealth;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void Respawn() 
    {
        var arena = Transform.FindObjectOfType<Arena>();
        var emptySpots = arena.GetEmptyGridSpots();

        // choose a random spot
        var spot = emptySpots[Random.Range(0,emptySpots.Count)];
        transform.position = arena.GridToWorldPosition(spot);

        IsDead = false;
        health = startingHealth;
        gameObject.SetActive(true);
    }

    public void Damage(float amount, GameObject damageSource = null)
    {
        health -= amount;

        if (health <= 0 && !IsDead)
        {
            IsDead = true;

            // report to Owner
            var owner = damageSource.GetOwner();
            if(owner != null)
            {
                Debug.Log(gameObject.name + " was killed by " + owner);
                var player = owner.GetComponent<PlayerControl>();
                if(player != null)
                {
                    player.Score += PointsValue;
                }
            }
            
            // explode
            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
            
            // play sound
            if(DeathSound != null)
                AudioSource.PlayClipAtPoint(DeathSound, transform.position);
            
            if(RespawnOnDeath)
            {
                gameObject.SetActive(false);
                GameBrain.Instance.WaitAndThenCall(RespawnDelaySeconds, () => Respawn());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
