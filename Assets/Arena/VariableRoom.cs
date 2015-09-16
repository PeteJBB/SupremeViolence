using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class VariableRoom: MonoBehaviour
{
    public Texture2D DefaultWallSkin;
    public bool SkinRightToEdge = false;

    private Transform gridContainer;
    private GameObject gridSquarePrefab;

    private Transform wallsContainer;
    private Transform floorsContainer;

    private Transform decoContainer; // for decorations

    private GridSquare[,] squares;

    public Vector2 RoomSize = new Vector2(5,5);

    [HideInInspector]
    public bool GenerateOnNextUpdate = false;

	// Use this for initialization
	void Start () 
    {
        GenerateRoom();
	}

    void Update()
    {
        if (GenerateOnNextUpdate)
        {
            GenerateOnNextUpdate = false;
            GenerateRoom();
        }
    }

    [ContextMenu("Show walls and floors in heirarchy")]
    public void ShowWallsInHeirarchy()
    {
        Helper.SetHideFlags(wallsContainer.gameObject, HideFlags.None);
        Helper.SetHideFlags(floorsContainer.gameObject, HideFlags.None);
    }

    [ContextMenu("Clear generated walls and floors")]
    public void ClearGeneratedWallsAndFloors()
    {
        ResolveContainers();
        Helper.DestroyAllChildren(wallsContainer, true);
        Helper.DestroyAllChildren(floorsContainer, true);
    }

    void GenerateGrid()
    {
        ResolveContainers();

        Helper.DestroyAllChildren(gridContainer, true);

        // create squares
        gridSquarePrefab = Resources.Load<GameObject>("Arena/grid_square");
        squares = new GridSquare[(int)RoomSize.x,(int)RoomSize.y];
        for(var x=0; x<RoomSize.x; x++)
        {
            for(var y=0; y<RoomSize.y; y++)
            {
                var sq = Instantiate(gridSquarePrefab).GetComponent<GridSquare>();
                sq.transform.SetParent(gridContainer);
                sq.SquareType = GridSquareType.Empty;
                var pos = new Vector3(x,y,0);
                sq.transform.localPosition = pos;
                squares[x,y] = sq;
            }
        }
    }

    void ResolveContainers()
    {
        if (gridContainer == null)
        {
            gridContainer = transform.Find("grid");
            if (gridContainer == null)
            {
                var obj = new GameObject();
                obj.name = "grid";
                gridContainer = obj.transform;
                gridContainer.SetParent(transform);
                gridContainer.localPosition = Vector3.zero;
            }
        }

        if (wallsContainer == null)
        {
            wallsContainer = transform.Find("walls");
            if (wallsContainer == null)
            {
                var obj = new GameObject();
                obj.name = "walls";
                wallsContainer = obj.transform;
                wallsContainer.SetParent(transform);
                wallsContainer.localPosition = Vector3.zero;
            }
        }

        if (floorsContainer == null)
        {
            floorsContainer = transform.Find("floors");
            if (floorsContainer == null)
            {
                var obj = new GameObject();
                obj.name = "floors";
                floorsContainer = obj.transform;
                floorsContainer.SetParent(transform);
                floorsContainer.localPosition = Vector3.zero;
            }
        }

        if (decoContainer == null)
        {
            decoContainer = transform.Find("deco");
            if (decoContainer == null)
            {
                var obj = new GameObject();
                obj.name = "deco";
                decoContainer = obj.transform;
                decoContainer.SetParent(transform);
                decoContainer.localPosition = Vector3.zero;
            }
        }

        gridContainer.gameObject.hideFlags = HideFlags.HideInHierarchy;
        Helper.SetHideFlags(wallsContainer.gameObject, HideFlags.HideInHierarchy);
        Helper.SetHideFlags(floorsContainer.gameObject, HideFlags.HideInHierarchy);
    }

    [ContextMenu("Generate room")]
    public void GenerateRoom()
    {
        ResolveContainers();
        ClearGeneratedWallsAndFloors();

        var walls = new List<Wall>();

        // interior walls and floors
        foreach(var sq in GetGridSquares())
        {
            if(sq.SquareType == GridSquareType.Wall)
                walls.Add(CreateWall(sq));
        }

        // update wall edges
        if (Helper.IsEditMode())
        {
            foreach(var wall in walls)
            {
                wall.UpdateEdges();
            }
        }

        gridContainer.hideFlags = HideFlags.HideInHierarchy;
        Helper.SetHideFlags(wallsContainer.gameObject, HideFlags.HideInHierarchy);
    }

    [ContextMenu("Reset room to default")]
    void GenerateAll()
    {
        GenerateGrid();
        GenerateRoom();
    }

    private Wall CreateWall(GridSquare sq)
    {
        var x = Mathf.RoundToInt(sq.transform.localPosition.x);
        var y = Mathf.RoundToInt(sq.transform.localPosition.y);

        var wall = Instantiate(GameSettings.WallPrefab);
        wall.transform.SetParent(wallsContainer);
        wall.transform.localPosition = new Vector3(x,y,0);

        wall.SkinBg = sq.WallSkinBg != null ? sq.WallSkinBg : DefaultWallSkin;
        wall.SkinTopLeft = sq.WallSkinTopLeft != null ? sq.WallSkinTopLeft : DefaultWallSkin;
        wall.SkinTopRight = sq.WallSkinTopRight != null ? sq.WallSkinTopRight : DefaultWallSkin;
        wall.SkinBottomLeft = sq.WallSkinBottomLeft != null ? sq.WallSkinBottomLeft : DefaultWallSkin;
        wall.SkinBottomRight = sq.WallSkinBottomRight != null ? sq.WallSkinBottomRight : DefaultWallSkin;
        
        return wall;
    }

    [ContextMenu("Reload all prefabs")]
    public void ReloadPrefabs()
    {
        var prefabs = Helper.GetComponentsInChildrenRecursive<PrefabLoader>(transform);
        foreach(var p in prefabs)
        {
            p.LoadPrefab();
        }
    }

    public GridSquare[] GetGridSquares()
    {
        ResolveContainers();
        return gridContainer.GetComponentsInChildren<GridSquare>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1, 0.1f);
        var offset = new Vector3(-0.5f, -0.5f, 0);
        Helper.DrawGizmoSquare(transform.position + offset + new Vector3(RoomSize.x/2f, RoomSize.y/2f, 0), RoomSize.x, RoomSize.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        var offset = new Vector3(-0.5f, -0.5f, 0);
        Helper.DrawGizmoSquare(transform.position + offset + new Vector3(RoomSize.x/2f, RoomSize.y/2f, 0), RoomSize.x, RoomSize.y);
    }

}