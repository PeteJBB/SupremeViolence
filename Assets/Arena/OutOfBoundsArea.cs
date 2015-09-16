using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[ExecuteInEditMode]
public class OutOfBoundsArea : MonoBehaviour
{
    public int Columns = 1;
    public int Rows = 1;
    public GameObject Prefab;

    private Transform container;


    //void Start()
    //{
    //    z_Columns = Columns;
    //    z_Rows = Rows;
    //}

    //[ContextMenu("Generate")]
    //void Generate()
    //{
    //    if(container == null)
    //    {
    //        container = transform.Find("container");
    //        if(container == null)
    //        {
    //            container = new GameObject().transform;
    //            container.gameObject.name = "container";
    //            container.SetParent(transform);
    //        }
    //    }

    //    Helper.SetHideFlags(container.gameObject, HideFlags.None);
    //    Helper.DestroyAllChildren(container, Helper.IsEditMode());

    //    for (var x = 0; x < Columns; x++)
    //    {
    //        for (var y = 0; y < Rows; y++)
    //        {
    //            var oob = Instantiate(Prefab).GetComponent<OutOfBounds>();
    //            oob.name = "outofbounds";
    //            oob.transform.SetParent(container);

    //            var pos = new Vector3(x, y, 0);
    //            oob.transform.localPosition = pos;
    //        }
    //    }

    //    Helper.SetHideFlags(container.gameObject, HideFlags.NotEditable | HideFlags.HideInHierarchy);
    //}

    //private int z_Columns;
    //private int z_Rows;

    //void Update()
    //{                
    //    if (Columns != z_Columns ||
    //        Rows != z_Rows
    //    )
    //    {
    //        z_Columns = Columns;
    //        z_Rows = Rows;
    //        Generate();
    //    }
    //}

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
