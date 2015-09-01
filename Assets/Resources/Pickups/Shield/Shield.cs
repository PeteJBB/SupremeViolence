using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using XInputDotNetPure;

public class Shield: Pickup, IDamageable, IReflector
{
    public AudioClip ActivateSound;
    public AudioClip RunningSound;
    public AudioClip DeactivateSound;
    public AudioClip HitSound;

    private SpriteRenderer overlay;
    private CircleCollider2D collider;
    //private FillBar ammoBar;
    
    private Material defaultMaterial;
    private Material flashMaterial;

    private bool isShieldActivated;
    private AudioSource shieldAudio;
    private float lastHitSoundPlayTime;
    private float lastAmmoDrain;
	
    public override string GetDescription()
    {
        return "A personal energy shield which reflects projectiles away from you and towards some other unsuspecting fool.";
    }

	// Use this for initialization
    void Awake()
    {        
        overlay = transform.FindChild("overlay").GetComponent<SpriteRenderer>();
        //ammoBar = transform.FindChild("ammobar").GetComponent<FillBar>();

        overlay.enabled = false;
        transform.localScale = Vector3.zero;

        collider = GetComponent<CircleCollider2D>();
        collider.enabled = false;

        defaultMaterial = overlay.material;
        flashMaterial = new Material(Shader.Find("Sprites/DefaultColorFlash"));
        flashMaterial.SetFloat("_FlashAmount", 0.75f);

        shieldAudio = gameObject.AddComponent<AudioSource>();
        shieldAudio.clip = RunningSound;
        shieldAudio.loop = true;
        shieldAudio.pitch = 0;

        base.OnPlayerPickup.AddListener(OnPlayerPickup);
	}

    void Update() 
    {
        if (Player != null)
        {
            if (isShieldActivated)
            {
                if (Time.time - lastAmmoDrain > 0.05f)
                {
                    Ammo--;
                    //ammoBar.SetFill(Ammo / (float)MaxAmmo);
                    lastAmmoDrain = Time.time;

                    if (Ammo <= 0)
                        DeactivateShield();
                }
            }

            var gamepadState = GetGamePadInput();

            if (gamepadState.Buttons.Y == ButtonState.Pressed && !isShieldActivated)
            {
                ActivateShield();
            }

            else if (gamepadState.Buttons.Y == ButtonState.Released && isShieldActivated)
            {
                DeactivateShield();
            }
        }
	}    

    private GamePadState GetGamePadInput()
    {
        switch(Player.PlayerIndex)
        {
            case 0:
            default:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.One);
            case 1:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Two);
            case 2:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Three);
            case 3:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Four);
        }
    }
    
    private void ActivateShield()
    {
        if (Ammo > 0)
        {
            Player.SetAmmoBarSource(this, new FillBarColorPoint[] { new FillBarColorPoint(new Color(.148f, .668f, 1), 0) });

            overlay.enabled = true;
            collider.enabled = true;
            isShieldActivated = true;
            transform.localScale = Vector3.zero;

            if (ActivateSound != null)
                AudioSource.PlayClipAtPoint(ActivateSound, transform.position);

            shieldAudio.Play();

            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.15f, "easetype", iTween.EaseType.easeOutCirc, "onupdate", (Action<object>)((val) =>
            {
                var a = (float)val;
                overlay.color = new Color(1, 1, 1, a);
                transform.localScale = new Vector3(a, a, a);
                shieldAudio.pitch = a;
                shieldAudio.volume = a;
            })));
        }
    }

    private void DeactivateShield()
    {
        isShieldActivated = false;

        Player.RestoreAmmoBarSource();
        
       if(DeactivateSound != null)
            AudioSource.PlayClipAtPoint(DeactivateSound, transform.position);

        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.15f, "easetype", iTween.EaseType.easeInCirc, "onupdate", (Action<object>)((val) =>
        {
            var a = (float)val;
            overlay.color = new Color(1,1,1, a);        
            transform.localScale = new Vector3(a, a, a);
            shieldAudio.pitch = a;
            shieldAudio.volume = a;
        }),
        "oncomplete", (Action)(() =>
        {
            overlay.enabled = false;
            collider.enabled = false;            
            shieldAudio.Stop();
        })));
        
    }

    public void OnPlayerPickup()
    {
        Physics2D.IgnoreCollision(collider, Player.GetComponent<Collider2D>());
    }

    public DamageResistances GetResistances()
    {
        return new DamageResistances()
        {
            Electrical = 0,
            Explosive = 0,
            Heat = 0,
            Kinetic = 0
        };
    }

    private float lastHitTime;
    public void Damage(float amount, GameObject damageSource = null)
    {
        // flash shield
        overlay.material = flashMaterial;
        lastHitTime = Time.time;
        Helper.Instance.WaitAndThenCall(0.1f, () =>
        {
            if(Time.time - lastHitTime >= 0.1f)
                overlay.material = defaultMaterial;
        });

        if (HitSound != null && Time.time - lastHitSoundPlayTime > 0.3f)
        {
            lastHitSoundPlayTime = Time.time;
            AudioSource.PlayClipAtPoint(HitSound, transform.position);
        }
    }

    public bool DoesReflectMe(GameObject obj)
    {
        return true;
    }
}