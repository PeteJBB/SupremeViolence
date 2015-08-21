using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Room: MonoBehaviour
{
    public Sprite FloorSprite;
    public Texture2D WallSkin;

    private Transform gridContainer;
    private GameObject gridSquarePrefab;

    private Transform wallsContainer;
    private Transform floorsContainer;

    private Transform decoContainer; // for decorations

    private GridSquare[,] squares;

    private const int roomSize = 5;

	// Use this for initialization
	void Start () 
    {
        GenerateWallsAndFloors();
	}

    //[ContextMenu("Re-generate from scratch")]
    void GenerateAll()
    {
        GenerateGrid();
        GenerateWallsAndFloors();
    }

    [ContextMenu("Show walls and floors in heirarchy")]
    public void ShowWallsInHeirarchy()
    {
        Helper.SetHideFlags(wallsContainer.gameObject, HideFlags.None);
    }

    [ContextMenu("Clear generated walls and floors")]
    public void ClearGeneratedWallsAndFloors()
    {
        ResolveContainers();
        Helper.DestroyAllChildren(wallsContainer, true);
    }

    void GenerateGrid()
    {
        ResolveContainers();

        Helper.DestroyAllChildren(gridContainer, true);

        // create squares
        gridSquarePrefab = Resources.Load<GameObject>("Arena/grid_square");
        squares = new GridSquare[roomSize,roomSize];
        for(var x=0; x<roomSize; x++)
        {
            for(var y=0; y<roomSize; y++)
            {
                var sq = Instantiate(gridSquarePrefab).GetComponent<GridSquare>();
                sq.transform.SetParent(gridContainer);
                sq.State = GridSquareState.Empty;
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

    [ContextMenu("Re-generate walls and floor from grid")]
    public void GenerateWallsAndFloors()
    {
        ResolveContainers();
        ClearGeneratedWallsAndFloors();

        var walls = new List<Wall>();

        // create external walls
        //if(RoomPosition == RoomPosition.DoorwayHorizontal)
        //{
        //    walls.Add(CreateWall(0,1));
        //    walls.Add(CreateWall(0,-1));
        //}
        //else if(RoomPosition == RoomPosition.DoorwayVertical)
        //{
        //    walls.Add(CreateWall(1,0));
        //    walls.Add(CreateWall(-1,0));
        //}
        //else
        //{
        //    for(var x = -1; x<=roomSize; x++)
        //    {
        //        for(var y = -1; y<=roomSize; y++)
        //        {
        //            if(x <0 || x == roomSize || y < 0 || y == roomSize)
        //            {
        //                // is there a door here?
        //                var doors = GetDoorPositions();
        //                if(!doors.Any(d => d.x == x && d.y == y))
        //                {
        //                    walls.Add(CreateWall(x,y));
        //                }
        //                //else
        //                //    CreateFloor(x,y);
        //            }
        //        }
        //    }
        //}
        
        // interior walls and floors
        foreach(var sq in gridContainer.GetComponentsInChildren<GridSquare>())
        {
            var x = Mathf.RoundToInt(sq.transform.localPosition.x);
            var y = Mathf.RoundToInt(sq.transform.localPosition.y);

            if(sq.State == GridSquareState.Wall)
                walls.Add(CreateWall(x,y));
            else 
                CreateFloor(x,y);
        }

        // update wall edges
        if (GameBrain.IsEditMode())
        {
            foreach(var wall in walls)
            {
                wall.GetComponent<Wall>().UpdateEdges();
            }
        }

        gridContainer.hideFlags = HideFlags.HideInHierarchy;
        Helper.SetHideFlags(wallsContainer.gameObject, HideFlags.HideInHierarchy);
    }

    //public void GenerateFloors()
    //{
    //    foreach(var sq in gridContainer.GetComponentsInChildren<GridSquare>())
    //    {
    //        var x = Mathf.RoundToInt(sq.transform.localPosition.x);
    //        var y = Mathf.RoundToInt(sq.transform.localPosition.y);

    //        if(sq.State == GridSquareState.Empty)
    //            CreateFloor(x,y);
    //    }
    //}

    private Wall CreateWall(int x, int y)
    {
        var wall = Instantiate(GameSettings.WallPrefab);
        wall.transform.SetParent(wallsContainer);
        wall.transform.localPosition = new Vector3(x,y,0);
        return wall;
    }

    private GameObject CreateFloor(int x, int y)
    {
        var floor = Instantiate(GameSettings.FloorPrefab);
        floor.transform.SetParent(wallsContainer);
        floor.transform.localPosition = new Vector3(x,y,0);

        if(FloorSprite != null)
            floor.GetComponent<SpriteRenderer>().sprite = FloorSprite;

        return floor;
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

    public int GetGridSizeX()
    {
        var squares = gridContainer.GetComponentsInChildren<GridSquare>();
        return (int)squares.Max(sq => sq.transform.localPosition.x) - (int)squares.Min(sq => sq.transform.localPosition.y);
    }

    public int GetGridSizeY()
    {
        var squares = gridContainer.GetComponentsInChildren<GridSquare>();
        return (int)squares.Max(sq => sq.transform.localPosition.y) - (int)squares.Min(sq => sq.transform.localPosition.y);
    }

    public GridSquare[] GetGridSquares()
    {
        ResolveContainers();
        return gridContainer.GetComponentsInChildren<GridSquare>();
    }

    //private List<Vector2> GetDoorPositions()
    //{
    //    var top = new Vector2(2, 5);
    //    var right = new Vector2(5, 2);
    //    var bottom = new Vector2(2, -1);
    //    var left = new Vector2(-1, 2);

    //    var list = new List<Vector2>();
    //    switch(RoomPosition)
    //    {
    //        case RoomPosition.TopLeft:
    //            list.Add(right);
    //            list.Add(bottom);
    //            break;
    //        case RoomPosition.TopMiddle:
    //            list.Add(right);
    //            list.Add(bottom);
    //            list.Add(left);
    //            break;
    //        case RoomPosition.TopRight:
    //            list.Add(bottom);
    //            list.Add(left);
    //            break;
    //        case RoomPosition.MiddleLeft:
    //            list.Add(top);
    //            list.Add(right);
    //            list.Add(bottom);
    //            break;
    //        case RoomPosition.Center:
    //            list.Add(top);
    //            list.Add(right);
    //            list.Add(bottom);
    //            list.Add(left);
    //            break;
    //        case RoomPosition.MiddleRight:
    //            list.Add(top);
    //            list.Add(bottom);
    //            list.Add(left);
    //            break;
    //        case RoomPosition.BottomLeft:
    //            list.Add(top);
    //            list.Add(right);
    //            break;
    //        case RoomPosition.BottomMiddle:
    //            list.Add(top);
    //            list.Add(right);
    //            list.Add(left);
    //            break;
    //        case RoomPosition.BottomRight:
    //            list.Add(top);
    //            list.Add(left);
    //            break;
    //    }

    //    return list;
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1, 0.1f);
        var offset = new Vector3(-0.5f, -0.5f, 0);
        Helper.DrawGizmoSquare(transform.position + offset + new Vector3(roomSize/2f, roomSize/2f, 0), roomSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        var offset = new Vector3(-0.5f, -0.5f, 0);
        Helper.DrawGizmoSquare(transform.position + offset + new Vector3(roomSize/2f, roomSize/2f, 0), roomSize);
    }

}

public enum RoomPosition
{
    Center,
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleRight,
    BottomRight,
    BottomMiddle,
    BottomLeft,
    MiddleLeft,
    DoorwayVertical,
    DoorwayHorizontal
}
