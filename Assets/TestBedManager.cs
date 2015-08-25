using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TestBedManager: MonoBehaviour 
{
    PlayerControl player;

	// Use this for initialization
	void Start () 
    {
        player = FindObjectOfType<PlayerControl>();
        
        foreach(var prefab in GameSettings.PickupPrefabs)
        {
            //Debug.Log("TestManager adding pickup: " + prefab.PickupName);
            var pickup = Instantiate(prefab);
            pickup.PickupSound = null;
            pickup.CollectPickup(player);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        foreach (var pu in player.Pickups)
        {
            pu.Ammo = pu.MaxAmmo;
        }
        //player.GetComponent<Damageable>().Health = 100;
	}

    void OnGUI()
    {
        GUI.TextArea(new Rect(20, 20, 200, 20), player.CurrentWeapon.PickupName + " " + player.CurrentWeapon.Ammo);
    }
}