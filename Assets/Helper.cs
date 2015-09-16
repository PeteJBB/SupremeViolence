using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using System.Linq;
using UnityEditor;

public class Helper : Singleton<Helper>
{
    void Awake()
    {

    }

	public static void DetachParticles(GameObject obj)
    {
        var particles = obj.transform.GetComponentsInChildren<ParticleSystem>();
        foreach(var p in particles)
        {
            p.transform.parent = null;
            p.Stop();
            Destroy(p.gameObject, p.startLifetime);
        }

        var trails = obj.transform.GetComponentsInChildren<TrailRenderer>();
        foreach(var t in trails)
        {
            t.transform.parent = null;
            Destroy(t.gameObject, t.time);
        }
    }

    //public static void FixLastTrailVertex(TrailRenderer trail, Vector3 point)
    //{
    //    trail.
    //}

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
    public static void DrawGizmoSquare(Vector3 center, float size = 1, float? sizey = null)
    {
        var halfSizeX = size / 2f;
        var halfSizeY = sizey == null ? halfSizeX : sizey.Value / 2f;
        DrawGizmoSquare(center.x - halfSizeX, center.y - halfSizeY, center.x + halfSizeX, center.y + halfSizeY, center.z);
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

        foreach(var g in children)
        {
            g.hideFlags = HideFlags.None;
            if(immediate)
                DestroyImmediate(g);
            else
                Destroy(g);
        }
    }

    public static List<T> GetComponentsRecursive<T>(Transform root)
    {
        var list = new List<T>();
        var inst = root.GetComponent<T>();
        if (inst != null)
            list.Add(inst);

        if(root.childCount > 0)
        {
            list.AddRange(GetComponentsInChildrenRecursive<T>(root));
        }

        return list;
    }

    public static List<T> GetComponentsInChildrenRecursive<T>(Transform root)
    {
        var list = new List<T>();
        if(root.childCount > 0)
        {
            list.AddRange(root.GetComponentsInChildren<T>());

            for(var i=0; i<root.childCount; i++)
            {
                list.AddRange(GetComponentsInChildrenRecursive<T>(root.GetChild(i)));
            }
        }

        return list;
    }

    
    public static T GetComponentInParentsRecursive<T>(Transform trans)
    {
        if(trans.parent != null)
        {
            var inst = trans.parent.GetComponent<T>();
            if (inst != null)
                return inst;

            return GetComponentInParentsRecursive<T>(trans.parent);
        }

        return default(T);
    }

    public static void DrawGridSquareGizmos(Vector3 pos, GridSquareType state, bool isSelected = false)
    {
        Color c;

        switch (state)
        {
            case GridSquareType.Wall:
                c = isSelected ? Color.green : Color.red;
                break;
            case GridSquareType.Void:
                c = isSelected ? Color.green : new Color(1,0,1);
                break;case GridSquareType.Empty:
            default:
                c = isSelected ? Color.green : Color.white;
                break;
        }
        Gizmos.color = new Color(c.r, c.g, c.b, 0.5f);
        var center = pos;// + new Vector3(0.5f, 0.5f, 0);
        Helper.DrawGizmoSquare(center, 1);

        Gizmos.color = new Color(c.r, c.g, c.b, 0.01f);
        Gizmos.DrawCube(center, new Vector3(1, 1, 0.005f));
        

    }

    public static void SetHideFlags(GameObject go, HideFlags flags, bool childrenOnly = false)
    {
        if(!childrenOnly)
            go.hideFlags = flags;

        foreach(Transform t in go.transform)
        {
            SetHideFlags(t.gameObject, flags);
        }
    }

    public static void DebugLogTime(string msg)
    {
        Debug.Log(Time.realtimeSinceStartup + ": " + msg);
    }
    public static void DebugLogTime(string format, params object[] args )
    {
        Debug.LogFormat(Time.realtimeSinceStartup + ": " + format, args);
    }


    public static GamePadState GetGamePadInput(int playerIndex)
    {
        switch(playerIndex)
        {
            case 0:
            default:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.One);
            case 1:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Two);
            case 2:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Three);
            case 3:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Four);
        }
    }

    public static void SetLayerRecursive(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    public static bool AnyAreNotNull(params object[] objects )
    {
        return objects.Any(x => x != null);
    }

    public static bool IsEditMode()
    {
        return !EditorApplication.isPlaying;
    }


    public static void SetDirtyRecursive(Transform transform)
    {
        EditorUtility.SetDirty(transform.gameObject);

        foreach(Transform t in transform)
        {
            EditorUtility.SetDirty(t.gameObject);
        }
    }
}
