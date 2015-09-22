using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class SpeedyBoots : Pickup
{
    ParticleSystem particles;
    Rigidbody2D ownerPlayerBody;

    //private FillBar ammoBar;
    
    private bool isBootsActive = false;
    private float lastAmmoDrain;

    public override string GetDescription()
    {
        return "This state-of-the-art footwear is surgically (and painfully) bonded to the feet and uses electric impulses to run your leg muscles at 130% power. That's gonna hurt tomorrow!";
    }

	// Use this for initialization
    void Awake()
    {
        particles = transform.FindChild("particles").GetComponent<ParticleSystem>();
        particles.enableEmission = false;

        base.OnPlayerPickup.AddListener(OnPlayerPickup_Handler);

        //ammoBar = transform.FindChild("ammobar").GetComponent<FillBar>();
        
	}

    void Start()
    {
        //ammoBar.Hide(true);
    }
	
	// Update is called once per frame
	void Update() 
    {
        if(isBootsActive)
        {
            particles.enableEmission = ownerPlayerBody.velocity.magnitude > 0.1f;

            if (Time.time - lastAmmoDrain > 0.05f)
            {
                Ammo--;
                //ammoBar.SetFill(Ammo / (float)MaxAmmo);
                lastAmmoDrain = Time.time;

                if (Ammo <= 0)
                    DeactivateBoots();
            }
        }

        if (Player != null)
        {
            var gamepadState = Helper.GetGamePadInput(Player.PlayerIndex);
            if (gamepadState.Buttons.B == ButtonState.Pressed && !isBootsActive)
            {
                ActivateBoots();
            }
            else if (gamepadState.Buttons.B == ButtonState.Released && isBootsActive)
            {
                DeactivateBoots();
            }
        }
	}

    private void OnPlayerPickup_Handler()
    {
        ownerPlayerBody = Player.GetComponent<Rigidbody2D>();
    }

    private void ActivateBoots()
    {
        if (Ammo > 0)
        {
            isBootsActive = true;
            particles.enableEmission = true;

            Player.SetAmmoBarSource(this, Color.red);
        }
    }

    private void DeactivateBoots()
    {
        isBootsActive = false;
        particles.enableEmission = false;        

        Player.RestoreAmmoBarSource();
    }

    public override float GetLegStrengthMultiplier()
    {
        return isBootsActive
            ? 1.6f
            : 1;
    }

    
}
