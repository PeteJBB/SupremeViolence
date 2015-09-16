using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[ExecuteInEditMode]
public class TiledSprite : MonoBehaviour
{
    public int Columns;
    public int Rows;
    public Sprite SourceSprite;
    public string SortingLayerName;
    public int SortOrder;
    public Material SpriteMaterial;

    private Transform container;

    void Start()
    {
        //GenerateTiles();

        z_Columns = Columns;
        z_Rows = Rows;
        z_SourceSprite = SourceSprite;
        z_SortingLayerName = SortingLayerName;
        z_SortOrder = SortOrder;
        z_SpriteMaterial = SpriteMaterial;
    }

    [ContextMenu("Generate Tiles")]
    void GenerateTiles()
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
                var spr = new GameObject().AddComponent<SpriteRenderer>();
                spr.name = "sprite";
                spr.sprite = SourceSprite;
                spr.transform.SetParent(container);

                var pos = new Vector3(x, y, 0);
                spr.transform.localPosition = pos;

                if(SpriteMaterial != null)
                    spr.material = SpriteMaterial;

                if(!String.IsNullOrEmpty(SortingLayerName))
                    spr.sortingLayerName = SortingLayerName;

                spr.sortingOrder = SortOrder;
            }
        }

        Helper.SetHideFlags(container.gameObject, HideFlags.NotEditable | HideFlags.HideInHierarchy); 
    }

    
    private int z_Columns;
    private int z_Rows;
    private Sprite z_SourceSprite;
    private string z_SortingLayerName;
    private int z_SortOrder;
    private Material z_SpriteMaterial;

    void Update()
    {
        if (Columns != z_Columns ||
            Rows != z_Rows ||
            SourceSprite != z_SourceSprite ||
            SortingLayerName != z_SortingLayerName ||
            SortOrder != z_SortOrder ||
            SpriteMaterial != z_SpriteMaterial
        )
        {
            GenerateTiles();

            z_Columns = Columns;
            z_Rows = Rows;
            z_SourceSprite = SourceSprite;
            z_SortingLayerName = SortingLayerName;
            z_SortOrder = SortOrder;
            z_SpriteMaterial = SpriteMaterial;
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
}
