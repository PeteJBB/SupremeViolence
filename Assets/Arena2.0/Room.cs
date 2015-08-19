using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Room: MonoBehaviour
{
    public RoomPosition RoomPosition;
    public Sprite FloorSprite;
    public Texture2D WallSkin;

    private Transform gridContainer;
    private GameObject gridSquarePrefab;

    private GameObject wallPrefab;
    private GameObject floorPrefab;

    private Transform wallsContainer;

    private GridSquare[,] squares;

	// Use this for initialization
	void Awake () 
    {
        gridContainer = transform.Find("grid");
        wallsContainer = transform.Find("walls");
	}

    [ContextMenu("Re-generate grid")]
    void GenerateGrid()
    {
        gridContainer = transform.Find("grid");
        if(gridContainer == null)
        {
            var obj = new GameObject();
            obj.name = "grid";
            gridContainer = obj.transform;
            gridContainer.SetParent(transform);
            gridContainer.localPosition = Vector3.zero;
        }

        Helper.DestroyAllChildren(gridContainer, true);

        gridSquarePrefab = Resources.Load<GameObject>("Arena/grid_square");

        squares = new GridSquare[5,5];

        for(var x=0; x<5; x++)
        {
            for(var y=0; y<5; y++)
            {
                var sq = Instantiate(gridSquarePrefab).GetComponent<GridSquare>();
                sq.transform.SetParent(gridContainer);
                var pos = new Vector3(x,y,0);
                sq.transform.localPosition = pos;
                squares[x,y] = sq;
            }
        }
    }

    [ContextMenu("Re-generate walls and floor from grid")]
    void GenerateWalls()
    {
        wallsContainer = transform.Find("walls");
        if(wallsContainer == null)
        {
            var obj = new GameObject();
            obj.name = "walls";
            wallsContainer = obj.transform;
            wallsContainer.SetParent(transform);
            wallsContainer.localPosition = Vector3.zero;
        }

        Helper.DestroyAllChildren(wallsContainer, true);

        wallPrefab = Resources.Load<GameObject>("Arena/Wall");
        floorPrefab = Resources.Load<GameObject>("Arena/Floor");

        var walls = new List<GameObject>();

        // create edges
        for(var x = -1; x<=5; x++)
        {
            for(var y = -1; y<=5; y++)
            {
                if(x <0 || x == 5 || y < 0 || y == 5)
                {
                    // is there a door here?
                    var doors = GetDoorPositions();
                    if(doors.Any(d => d.x == x && d.y == y))
                        CreateFloor(x,y);
                    else
                    {
                        walls.Add(CreateWall(x,y));
                    }
                }
            }
        }

        // interior walls if there are any
        foreach(var sq in gridContainer.GetComponentsInChildren<GridSquare>())
        {
            var x = Mathf.RoundToInt(sq.transform.localPosition.x);
            var y = Mathf.RoundToInt(sq.transform.localPosition.y);

            if(sq.State == GridSquareState.Wall)
            {
                walls.Add(CreateWall(x,y));
            }
            else
            {
                // floor
                CreateFloor(x,y);
            }
        }

        // update wall edges
        foreach(var wall in walls)
        {
            wall.GetComponent<Wall>().UpdateEdges();
        }
    }

    private GameObject CreateWall(int x, int y)
    {
        var wall = Instantiate(wallPrefab);
        wall.transform.SetParent(wallsContainer);
        wall.transform.localPosition = new Vector3(x,y,0);
        return wall;
    }

    private GameObject CreateFloor(int x, int y)
    {
        var floor = Instantiate(floorPrefab);
        floor.transform.SetParent(wallsContainer);
        floor.transform.localPosition = new Vector3(x,y,0);
        
        if(FloorSprite != null)
            floor.GetComponent<SpriteRenderer>().sprite = FloorSprite;

        return floor;
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
        return gridContainer.GetComponentsInChildren<GridSquare>();
    }

    private List<Vector2> GetDoorPositions()
    {
        var list = new List<Vector2>();
        switch(RoomPosition)
        {
            case RoomPosition.TopLeft:
                list.Add(new Vector2(5, 2)); // right
                list.Add(new Vector2(2, -1)); // bottom
                break;
            case RoomPosition.TopMiddle:
                list.Add(new Vector2(5, 2)); // right
                list.Add(new Vector2(2, -1)); // bottom
                list.Add(new Vector2(0, 2)); // left
                break;
            case RoomPosition.TopRight:
                list.Add(new Vector2(2, -1)); // bottom
                list.Add(new Vector2(-1, 2)); // left
                break;
            case RoomPosition.MiddleLeft:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(5, 2)); // right
                list.Add(new Vector2(2, -1)); // bottom
                break;
            case RoomPosition.Center:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(5, 2)); // right
                list.Add(new Vector2(2, -1)); // bottom
                list.Add(new Vector2(-1, 2)); // left
                break;
            case RoomPosition.MiddleRight:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(2, -1)); // bottom
                list.Add(new Vector2(0, 2)); // left
                break;
            case RoomPosition.BottomLeft:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(5, 2)); // right
                break;
            case RoomPosition.BottomMiddle:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(5, 2)); // right
                list.Add(new Vector2(-1, 2)); // left
                break;
            case RoomPosition.BottomRight:
                list.Add(new Vector2(2, 5)); // top
                list.Add(new Vector2(-1, 2)); // left
                break;
        }

        return list;
    }

//    void DrawDoorGizmo(string pos)
//    {
//        var roomSize = 5f;
//        if(pos == "top")
//            Gizmos.DrawCube(transform.position + new Vector3(2f, 4.45f, 0), new Vector3(1, 0.1f, 1));
//        else if(pos == "right")
//            Gizmos.DrawCube(transform.position + new Vector3(4.45f, 2f, 0), new Vector3(0.1f, 1, 1));
//        else if(pos == "bottom")
//            Gizmos.DrawCube(transform.position + new Vector3(2f, -0.45f, 0), new Vector3(1, 0.1f, 1));
//        else if(pos == "left")
//            Gizmos.DrawCube(transform.position + new Vector3(-0.45f, 2f, 0), new Vector3(0.1f, 1, 1));
//    }

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