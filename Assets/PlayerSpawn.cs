using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerSpawn : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "PlayerSpawn.png", true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position, "PlayerSpawn_selected.png", true);
    }
}
