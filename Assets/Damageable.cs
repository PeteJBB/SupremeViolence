using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour 
{
    public float StartingHealth = 100;
    public float Health;
    public GameObject ExplosionPrefab;
    public GameObject CorpsePrefab;

    public bool RespawnOnDeath = false;
    public float RespawnDelaySeconds = 1;

    public AudioClip DeathSound;
    public int PointsValue = 0;

    public bool IsDead = false;

	// Use this for initialization
	void Start () 
    {
        Health = StartingHealth;
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
        Health = StartingHealth;
        gameObject.SetActive(true);
    }

    public void Damage(float amount, GameObject damageSource = null)
    {
        Health -= amount;

        if (Health <= 0 && !IsDead)
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
            if(ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            }

            // corpse
            if(CorpsePrefab != null)
            {
                Instantiate(CorpsePrefab, transform.position, transform.rotation);
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
