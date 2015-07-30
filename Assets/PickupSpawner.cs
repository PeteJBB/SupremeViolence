using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour 
{
    public GameObject[] PickupPool;
    public float SpawnRate = 0.05f; // chance that an item spawns every second

    private float lastSpawnCheck = 0;

	// Use this for initialization
	void Start () 
    {
	    // spawn one of each item at random free location
        var emptySpots = Arena.Instance.GetEmptyGridSpots();
        for(var i = 0; i< PickupPool.Length; i++)
		{
            if(emptySpots.Count == 0)
            {
                Debug.Log("Ran out of empty places to spawn items");
                break;
            }

            var spot = emptySpots[Random.Range(0,emptySpots.Count)];
            var instance = (GameObject)Instantiate(PickupPool[i], Arena.Instance.GridToWorldPosition(spot), Quaternion.identity);
            Arena.Instance.SetGridObject(spot, instance);
            emptySpots.Remove(spot);
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameBrain.Instance.State == GameState.GameOn && Time.time - lastSpawnCheck > 1)
        {
            if(Random.Range(0f,1f) < SpawnRate)
            {
                var pickup = PickupPool[Random.Range(0, PickupPool.Length)];
                var emptySpots = Arena.Instance.GetEmptyGridSpots();
                if(emptySpots.Count > 0)
                {
                    var spot = emptySpots[Random.Range(0, emptySpots.Count)];
                    var instance = (GameObject)Instantiate(pickup, Arena.Instance.GridToWorldPosition(spot), Quaternion.identity);
                    Arena.Instance.SetGridObject(spot, instance);
                }

            }

            lastSpawnCheck = Time.time;
        }
	}
}
