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

        base.OnPlayerPickup.AddListener(OnPlayerPickup);

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
            var gamepadState = GetGamePadInput();
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

    private void OnPlayerPickup()
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
