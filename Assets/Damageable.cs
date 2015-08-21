using UnityEngine;
using System.Collections;
using System.Linq;

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
        if(GameBrain.Instance != null && GameBrain.Instance.State == PlayState.GameOn)
        {
            var emptySquares = Arena2.Instance.GetEmptyGridSquares();

            // choose a random spot
            var spot = emptySquares[Random.Range(0,emptySquares.Count)];
            transform.position = Arena2.GridToWorldPosition(spot.x, spot.y);

            IsDead = false;
            Health = StartingHealth;
            gameObject.SetActive(true);
        }
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
                    var plInfo = GameState.Players[player.PlayerIndex];
                    plInfo.RoundScore += PointsValue;
                    plInfo.TotalScore += PointsValue;
                    plInfo.Cash += PointsValue * GameSettings.CashForKill;
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
                var corpse = (GameObject)Instantiate(CorpsePrefab, transform.position, transform.rotation);
                var oriented = Helper.GetComponentsInChildrenRecursive<OrientedSprite>(transform);
                if(oriented.Any())
                    corpse.BroadcastMessage("SetOrientation", oriented.First().orientation, SendMessageOptions.DontRequireReceiver);
            }
            
            // play sound
            if(DeathSound != null)
                AudioSource.PlayClipAtPoint(DeathSound, transform.position);
            
            if(RespawnOnDeath)
            {
                gameObject.SetActive(false);
                Helper.Instance.WaitAndThenCall(RespawnDelaySeconds, () => Respawn());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
