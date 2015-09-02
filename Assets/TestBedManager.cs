using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TestBedManager: MonoBehaviour 
{
    PlayerControl[] players;

    public bool GiveAllPickups;
    public bool UnlimitedAmmo;
    public bool Invincibility;

	// Use this for initialization
	void Start () 
    {
        players = FindObjectsOfType<PlayerControl>();

        if (GiveAllPickups)
        {
            foreach (var player in players)
            {
                foreach (var prefab in GameSettings.PickupPrefabs)
                {
                    //Debug.Log("TestManager adding pickup: " + prefab.PickupName);
                    var pickup = Instantiate(prefab);
                    pickup.PickupSound = null;
                    pickup.CollectPickup(player);
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (UnlimitedAmmo)
        {
            foreach (var player in players)
            {
                foreach (var pu in player.Pickups)
                {
                    pu.Ammo = pu.MaxAmmo;
                }
            }
        }

        if (Invincibility)
        {
            foreach (var player in players)
            {
                player.GetComponent<Damageable>().Health = 100;
            }
        }
	}

    void OnGUI()
    {
        //GUI.TextArea(new Rect(20, 20, 200, 20), player.CurrentWeapon.PickupName + " " + player.CurrentWeapon.Ammo);
    }
}