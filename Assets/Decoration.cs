using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Decoration : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    /// <summary>
    /// Check if this decoration can spawn in the given square
    /// Returns null if it can't or doesn't want to spawn here
    /// </summary>
    public virtual Vector3? GetSpawnLocationForGridSquare(int gridx, int gridy, List<GameObject> items)
    {
        return null;
    }
}
