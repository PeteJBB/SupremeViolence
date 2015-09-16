using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[ExecuteInEditMode]
public class TiledWall : MonoBehaviour
{
    public int Columns = 1;
    public int Rows = 1;
    private Transform container;

    public Texture2D SkinBg;
    public Texture2D SkinTopLeft;
    public Texture2D SkinTopRight;
    public Texture2D SkinBottomLeft;
    public Texture2D SkinBottomRight;

    void Start()
    {
        //if (!IsGenerated)
        //    GenerateWalls();

        z_Columns = Columns;
        z_Rows = Rows;

        z_SkinBg = SkinBg;
        z_SkinTopLeft = SkinTopLeft;
        z_SkinTopRight = SkinTopRight;
        z_SkinBottomLeft = SkinBottomLeft;
        z_SkinBottomRight = SkinBottomRight;
    }

    private bool IsGenerated
    {
        get
        {
            if (container != null)
                return true;

           container = transform.Find("container");
           return container != null;
        }
    }

    [ContextMenu("Generate Walls")]
    void GenerateWalls()
    {
        if(container == null)
        {
            container = transform.Find("container");
            if(container == null)
            {
                container = new GameObject().transform;
                container.gameObject.name = "container";
                container.SetParent(transform);
            }
        }

        Helper.SetHideFlags(container.gameObject, HideFlags.None);
        Helper.DestroyAllChildren(container, Helper.IsEditMode());

        for (var x = 0; x < Columns; x++)
        {
            for (var y = 0; y < Rows; y++)
            {
                var wall = Instantiate(GameSettings.WallPrefab).GetComponent<Wall>();
                wall.name = "wall";
                wall.transform.SetParent(container);

                var pos = new Vector3(x, y, 0);
                wall.transform.localPosition = pos;

                wall.SkinBg = SkinBg;
                wall.SkinTopLeft = SkinTopLeft;
                wall.SkinTopRight = SkinTopRight;
                wall.SkinBottomLeft = SkinBottomLeft;
                wall.SkinBottomRight = SkinBottomRight;
            }
        }

        Helper.SetHideFlags(container.gameObject, HideFlags.NotEditable | HideFlags.HideInHierarchy);

        Wall.UpdateAllWallEdges();
    }

    private int z_Columns;
    private int z_Rows;

    private Texture2D z_SkinBg;
    private Texture2D z_SkinTopLeft;
    private Texture2D z_SkinTopRight;
    private Texture2D z_SkinBottomLeft;
    private Texture2D z_SkinBottomRight;

    void Update()
    {                
        if (Columns != z_Columns ||
            Rows != z_Rows ||
            SkinBg != z_SkinBg ||
            SkinTopLeft != z_SkinTopLeft ||
            SkinTopRight != z_SkinTopRight ||
            SkinBottomLeft != z_SkinBottomLeft ||
            SkinBottomRight != z_SkinBottomRight
        )
        {
            z_Columns = Columns;
            z_Rows = Rows;

            z_SkinBg = SkinBg;
            z_SkinTopLeft = SkinTopLeft;
            z_SkinTopRight = SkinTopRight;
            z_SkinBottomLeft = SkinBottomLeft;
            z_SkinBottomRight = SkinBottomRight;
            
            GenerateWalls();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,0,0);
        var size = new Vector3(Columns, Rows, 0);
        Gizmos.DrawCube(transform.position + (size / 2f) - new Vector3(0.5f, 0.5f, 0), size);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,1,1,1);
        var size = new Vector3(Columns, Rows, 0);
        Helper.DrawGizmoSquare(transform.position + (size / 2f) - new Vector3(0.5f, 0.5f, 0), size.x, size.y);
    }

    [ContextMenu("Update All Wall Edges")]
    public void UpdateAllWallEdgesNow()
    {
        Wall.UpdateAllWallEdges();
    }
}
