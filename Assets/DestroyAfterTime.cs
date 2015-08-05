using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour 
{
    public float Seconds = 1;
    private float birthday;

	// Use this for initialization
	void Start () 
    {
        birthday = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Time.time - birthday > Seconds)
        {
            Helper.DetachParticles(gameObject);
            Destroy(gameObject);
        }
	}
}
