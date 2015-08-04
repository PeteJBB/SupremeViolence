using UnityEngine;
using System.Collections;

public class Helper : MonoBehaviour 
{

	public static void DetachParticles(GameObject obj)
    {
        var particles = obj.transform.GetComponentsInChildren<ParticleSystem>();//.FindChild("Particles");
        foreach(var p in particles)
        {
            p.transform.parent = null;
            p.Stop();
            Destroy(p.gameObject, p.startLifetime);
        }
    }
}
