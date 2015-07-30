using UnityEngine;
using System.Collections;

public class Helper : MonoBehaviour 
{

	public static void DetachParticles(GameObject obj)
    {
        var particles = obj.transform.GetComponentsInChildren<ParticleSystem>();//.FindChild("Particles");
        foreach(var part in particles)
        {
            part.transform.parent = null;
            var pSys = part.GetComponent<ParticleSystem>();
            pSys.Stop();
            Destroy(part.gameObject, pSys.startLifetime);
        }
    }
}
