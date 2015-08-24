using UnityEngine;
using UnityEngine.UI;


public class ChanceToSpawn: MonoBehaviour 
{
    public float Chance = 1f;

	// Use this for initialization
	void Start () 
    {
        if (Random.value > Mathf.Min(Chance, 1))
            Destroy(gameObject);
	}
}