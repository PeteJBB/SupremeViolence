using UnityEngine;
using System.Collections;

public class SpeedyBoots : Pickup
{
	// Use this for initialization
	void Start()
    {
        BaseStart();
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}

    public override float GetMoveMultiplier()
    {
        return 2;
    }

    public override bool CanPlayerPickup(PlayerControl player)
    {
        // look for another speedy boots
        foreach(var p in player.Pickups)
        {
            if(p is SpeedyBoots)
                return false;
        }

        return true;
    }
}
