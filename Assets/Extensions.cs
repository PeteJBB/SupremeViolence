using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions 
{
    // keep track of the owner of each object
    // owner is not Parent, it is mostly used for tracking who fired projectiles
    public static void SetOwner(this GameObject obj, GameObject owner)
    {
        if(obj != null)
        {
            var ownedObj = obj.GetComponent<OwnedObject>();
            if(ownedObj == null)
                ownedObj = obj.AddComponent<OwnedObject>();

            ownedObj.Owner = owner;
        }
    }

    public static GameObject GetOwner(this GameObject obj)
    {
        if(obj == null)
            return null;

        var ownedObj = obj.GetComponent<OwnedObject>();
        if(ownedObj != null)
            return ownedObj.Owner;

        return null;
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 ToVector3(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
}
