using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameObjectExtensions 
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
}
