﻿using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour 
{
    public Pickup[] PickupPool;

	// Use this for initialization
	void Start () 
    {
	    // spawn items at random locations
		for(var i = 0; i< PickupPool.Length; i++)
		{
	        var x = Random.Range(-4, 4);
	        var y = Random.Range(-4, 4);
	        Instantiate(PickupPool[i], new Vector3(x,y,0), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
