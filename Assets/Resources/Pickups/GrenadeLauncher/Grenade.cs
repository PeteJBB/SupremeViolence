using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour 
{
    public GameObject ExplosionPrefab;

    private float fuseTime = 3;
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
                var exp = (GameObject)Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                exp.SetOwner(gameObject.GetOwner());
            }

            // detach particle sys
            var particles = transform.FindChild("Particles");
            particles.transform.parent = null;
            var pSys = particles.GetComponent<ParticleSystem>();
            pSys.Stop();
            Destroy(particles.gameObject, pSys.startLifetime);

            Destroy(gameObject);
        }
	}
}
