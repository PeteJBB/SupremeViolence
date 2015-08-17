using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

    public static void DrawGizmoSquare(float minx, float miny, float maxx, float maxy, float z = 0)
    {
        DrawGizmoSquare(new Vector3(minx, miny, z), new Vector3(maxx, maxy, z));
    }
    public static void DrawGizmoSquare(Vector3 center, float size = 1)
    {
        var halfSize = size / 2f;
        DrawGizmoSquare(center.x - halfSize, center.y - halfSize, center.x + halfSize, center.y + halfSize, center.z);
    }
    public static void DrawGizmoSquare(Vector3 min, Vector3 max)
    {
        Gizmos.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, max.z));
        Gizmos.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, min.y, max.z));
        Gizmos.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(min.x, min.y, max.z));
        Gizmos.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, max.z));
    }

    public static void DestroyAllChildren(Transform t, bool immediate = false)
    {
        var children = new List<GameObject>();
        for(var i=0; i<t.childCount; i++)
            children.Add(t.GetChild(i).gameObject);

        t.DetachChildren();
        foreach(var g in children)
        {
            if(immediate)
                DestroyImmediate(g);
            else
                Destroy(g);
        }
    }
}
