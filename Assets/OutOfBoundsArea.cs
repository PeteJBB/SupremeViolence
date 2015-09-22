using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class OutOfBoundsArea : MonoBehaviour
{
    public int Columns = 1;
    public int Rows = 1;

    private Transform container;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.1f);
        var size = new Vector3(Columns, Rows, 0);
        Gizmos.DrawCube(transform.position + (size / 2f) - new Vector3(0.5f, 0.5f, 0), size);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,1,1,1);
        var size = new Vector3(Columns, Rows, 0);
        Helper.DrawGizmoSquare(transform.position + (size / 2f) - new Vector3(0.5f, 0.5f, 0), size.x, size.y);
    }

    public static bool IsOutOfBounds(int x, int y, OutOfBoundsArea[] arr = null)
    {
        if(arr == null)
            arr = FindObjectsOfType<OutOfBoundsArea>();

        return arr.Any(o => o.transform.position.x <= x
            && o.transform.position.y <= y
            && o.transform.position.x + (o.Columns-1) >= x
            && o.transform.position.y + (o.Rows-1) >= y);
    }
}
