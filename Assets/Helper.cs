using UnityEngine;
using System;
using System.Collections;

public class Helper : MonoBehaviour 
{
    private static Helper _instance;
    public static Helper Instance
    {
        get 
        { 
            if(_instance == null)
                _instance = new GameObject().AddComponent<Helper>();
            return _instance; 
        }
    }

    void Awake()
    {

    }

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

    public void WaitAndThenCall(float waitSeconds, Action funcToRun)
    {
        StartCoroutine(CoWaitAndThenCall(waitSeconds, funcToRun));
    }
    
    public IEnumerator CoWaitAndThenCall(float waitSeconds, Action funcToRun)
    {
        yield return new WaitForSeconds(waitSeconds);
        funcToRun();
    }
}
