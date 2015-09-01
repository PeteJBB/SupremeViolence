using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class Damageable : MonoBehaviour, IDamageable
{
    public float StartingHealth = 100;
    public float Health;

    public GameObject ExplosionPrefab;
    public GameObject CorpsePrefab;
    

    public bool RespawnOnDeath = false;
    public float RespawnDelaySeconds = 1;

    public AudioClip DeathSound;
    public int PointsValue = 0;

    public bool FlashOnDamage = false;
    public bool IsDead = false;

    public DamageResistances Resistances;

    private FillBar healthBar;
    private List<SpriteRenderer> spriteRenderers;
    private Dictionary<int, Material> defaultMaterials;
    private Material flashMaterial;
    private bool flashNextUpdate = false;

	// Use this for initialization
	void Start () 
    {
        Health = StartingHealth;
        healthBar = transform.GetComponentInChildren<FillBar>();

        flashMaterial =  new Material(Shader.Find("Sprites/DefaultColorFlash"));
        flashMaterial.SetFloat("_FlashAmount", 0.75f);
        spriteRenderers = Helper.GetComponentsInChildrenRecursive<SpriteRenderer>(transform);
        defaultMaterials = new Dictionary<int, Material>();
        foreach (var sr in spriteRenderers)
        {
            var key = sr.gameObject.GetInstanceID();
            if(!defaultMaterials.ContainsKey(key))
                defaultMaterials.Add(key, sr.material);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (flashNextUpdate)
        {
            flashNextUpdate = false;
            FlashSprites();
        }
	}

    public DamageResistances GetResistances()
    {
        return Resistances;
    }

    public void Respawn() 
    {
        if(GameBrain.Instance != null && GameBrain.Instance.State == PlayState.GameOn)
        {
            var emptySquares = Arena.Instance.GetEmptyGridSquares();

            // choose a random spot
            var spot = emptySquares[Random.Range(0,emptySquares.Count)];
            transform.position = Arena.GridToWorldPosition(spot.x, spot.y);

            IsDead = false;
            Health = StartingHealth;
            gameObject.SetActive(true);

            healthBar.SetFill(1);
            ResetAllMaterials();
        }
    }

    public void Damage(float amount, GameObject damageSource = null)
    {
        Health -= amount;
        
        if (healthBar != null)
        {
            healthBar.SetFill(Health / StartingHealth);
        }

        if (Health <= 0 && !IsDead)
        {
            IsDead = true;

            // report to Owner
            var owner = damageSource.GetOwner();
            if (owner != null)
            {
                Debug.Log(gameObject.name + " was killed by " + owner);
                var player = owner.GetComponent<PlayerControl>();
                if (player != null)
                {
                    var plInfo = GameState.Players[player.PlayerIndex];
                    plInfo.RoundScore += PointsValue;
                    plInfo.TotalScore += PointsValue;
                    plInfo.Cash += PointsValue * GameSettings.CashForKill;
                }
            }

            // explode
            if (ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            }

            // corpse
            if (CorpsePrefab != null)
            {
                var corpse = (GameObject)Instantiate(CorpsePrefab, transform.position, transform.rotation);
                var oriented = Helper.GetComponentsInChildrenRecursive<OrientedSprite>(transform);
                if (oriented.Any())
                    corpse.BroadcastMessage("SetOrientation", oriented.First().orientation, SendMessageOptions.DontRequireReceiver);
            }

            // play sound
            if (DeathSound != null)
                AudioSource.PlayClipAtPoint(DeathSound, transform.position);

            if (RespawnOnDeath)
            {
                gameObject.SetActive(false);
                Helper.Instance.WaitAndThenCall(RespawnDelaySeconds, () => Respawn());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            flashNextUpdate = FlashOnDamage;
        }
    }

    public void SetHealth(float val)
    {
        Health = val;

        if(healthBar != null)
            healthBar.SetFill(Health / StartingHealth);
    }

    public void FlashSprites()
    {
        if (spriteRenderers.Any())
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null && sr.enabled && sr.sortingLayerName == "Objects")
                {
                    StartCoroutine(FlashSprite(sr));
                }
            }
        }
    }

    private void ResetAllMaterials()
    {
        foreach (var sr in spriteRenderers)
        {
            var key = sr.gameObject.GetInstanceID();
            if (defaultMaterials.ContainsKey(key))
            {
                sr.material = defaultMaterials[key];
            }
        }
}

    public IEnumerator FlashSprite(SpriteRenderer sr)
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(0.1f);         
        sr.material = defaultMaterials[sr.gameObject.GetInstanceID()];
    }
}

[System.Serializable]
public class DamageResistances
{
    public float Kinetic = 0;
    public float Heat = 0;
    public float Explosive = 0;
    public float Electrical = 0;
}

public interface IDamageable
{
    DamageResistances GetResistances();
    void Damage(float amount, GameObject damageSource = null);
}