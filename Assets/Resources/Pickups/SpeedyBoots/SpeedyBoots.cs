using UnityEngine;
using System.Collections;

public class SpeedyBoots : Pickup
{
    ParticleSystem particles;
    Rigidbody2D ownerPlayerBody;

	// Use this for initialization
    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.enableEmission = false;
	}
	
	// Update is called once per frame
	void Update() 
    {
        if(ownerPlayerBody != null)
        {
            particles.enableEmission = ownerPlayerBody.velocity.magnitude > 0.1f;
        }
	}

    public override float GetLegStrengthMultiplier()
    {
        return 1.3f;
    }

    public override string GetDescription()
    {
        return "This state-of-the-art footwear is surgically (and painfully) bonded to the feet and uses electric impulses to run your leg muscles at 130% power. That's gonna hurt tomorrow!";
    }

    public override void OnPlayerPickup(PlayerControl player)
    {
        ownerPlayerBody = player.GetComponent<Rigidbody2D>();
        particles.enableEmission = true;

        if(GameBrain.Instance.State == PlayState.GameOn && PlayerHudCanvas.Instance != null)
            PlayerHudCanvas.Instance.ShowPickupText(this.GetPickupName(), player.gameObject, player.PlayerIndex);
    }
}
