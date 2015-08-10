using UnityEngine;
using System.Collections;

public class SpeedyBoots : Pickup
{
    ParticleSystem particles;
    Rigidbody2D ownerPlayerBody;

	// Use this for initialization
	void Start()
    {
        particles = GetComponent<ParticleSystem>();
        particles.enableEmission = false;

        BaseStart();
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
        return 1.5f;
    }

    public override void OnPlayerPickup(PlayerControl player)
    {
        ownerPlayerBody = player.GetComponent<Rigidbody2D>();
        particles.enableEmission = true;

        if(GameBrain.Instance.State == GameState.GameOn)
            MainCanvas.Instance.ShowPickupText(this.Name, player.gameObject, (int)player.PlayerIndex + 1);
    }
}
