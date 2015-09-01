using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour 
{
    public GameObject ExplosionPrefab;

    private float fuseTime = 2;
    private float birthday;

	// Use this for initialization
	void Start () 
    {
        birthday = Time.time;
	}

	// Update is called once per frame
	void Update () 
    {
	    if(Time.time - birthday > fuseTime)
        {
            // explode!
            if(ExplosionPrefab != null)
            {
                var exp = Instantiate<GameObject>(ExplosionPrefab).GetComponent<Explosion>();
                exp.transform.position = transform.position;
                exp.gameObject.SetOwner(gameObject.GetOwner());
            }

            Helper.DetachParticles(gameObject);
            Destroy(gameObject);
        }
	}
}
