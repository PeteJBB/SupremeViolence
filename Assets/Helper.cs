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

    public static void DrawGridSquareGizmos(Vector3 pos, GridSquareState state, DoorPosition doorPos)
    {
        Color c = Color.white;
        switch(state)
        {
            case GridSquareState.Void:
                c = Color.red;
                break;
            case GridSquareState.Room:
                c = Color.green;
                break;
            case GridSquareState.Hallway:
                c = Color.cyan;
                break;
        }
        
        Gizmos.color = c;
        Helper.DrawGizmoSquare(pos, 1);

        switch(doorPos)
        {
            case DoorPosition.Top:
                Gizmos.DrawCube(pos + new Vector3(0, 0.45f, 0), new Vector3(1, 0.1f, 1));
                break;
            case DoorPosition.Right:
                Gizmos.DrawCube(pos + new Vector3(0.45f, 0, 0), new Vector3(0.1f, 1, 1));
                break;
            case DoorPosition.Bottom:
                Gizmos.DrawCube(pos + new Vector3(0, -0.45f, 0), new Vector3(1, 0.1f, 1));
                break;
            case DoorPosition.Left:
                Gizmos.DrawCube(pos + new Vector3(-0.45f, 0, 0), new Vector3(0.1f, 1, 1));
                break;
        }

        Gizmos.color = new Color(c.r, c.g, c.b, 0.1f);
        Gizmos.DrawCube(pos, new Vector3(1, 1, 0.01f));
        

    }
}
