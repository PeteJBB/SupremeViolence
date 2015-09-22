using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PickupSpawn : MonoBehaviour 
{
	void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "PickupSpawn.png", true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position, "PickupSpawn_selected.png", true);
    }
}
