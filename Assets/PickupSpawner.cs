using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour 
{
    public GameObject[] PickupPool;

    private Arena arena;

	// Use this for initialization
	void Start () 
    {
        arena = GetComponent<Arena>();

	    // spawn one of each item at random free location
        var emptySpots = arena.GetEmptyGridSpots();
        for(var i = 0; i< PickupPool.Length; i++)
		{
            if(emptySpots.Count == 0)
            {
                Debug.Log("Ran out of empty places to spawn items");
                break;
            }

            var spot = emptySpots[Random.Range(0,emptySpots.Count)];
	        var instance = (GameObject)Instantiate(PickupPool[i], arena.GridToWorldPosition(spot), Quaternion.identity);
            arena.SetGridObject(spot, instance);
            emptySpots.Remove(spot);
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}
}
