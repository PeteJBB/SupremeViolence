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

    private Transform wallsContainer;

    private GridSquare[,] squares;

    private int roomSize;
    private bool isDoorway;

	// Use this for initialization
	void Awake () 
    {
        gridContainer = transform.Find("grid");
        wallsContainer = transform.Find("walls");
	}

    [ContextMenu("Re-generate from scratch")]
    void GenerateAll()
    {
        GenerateGrid();
        GenerateWallsAndFloors();
    }

    [ContextMenu("Clear generated walls and floors")]
    public void ClearGeneratedWallsAndFloors()
    {
        ResolveContainers();
        Helper.DestroyAllChildren(wallsContainer, true);
    }

    void GenerateGrid()
    {
        isDoorway = RoomPosition == RoomPosition.DoorwayHorizontal || RoomPosition == RoomPosition.DoorwayVertical;
        roomSize = isDoorway ? 1 : 5;

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
        gridContainer = transform.Find("grid");
        if(gridContainer == null)
        {
            var obj = new GameObject();
            obj.name = "grid";
            gridContainer = obj.transform;
            gridContainer.SetParent(transform);
            gridContainer.localPosition = Vector3.zero;
        }

        wallsContainer = transform.Find("walls");
        if(wallsContainer == null)
        {
            var obj = new GameObject();
            obj.name = "walls";
            wallsContainer = obj.transform;
            wallsContainer.SetParent(transform);
            wallsContainer.localPosition = Vector3.zero;
        }
    }

    [ContextMenu("Re-generate walls and floor from grid")]
    public void GenerateWallsAndFloors(bool generateExternalWalls = true) // external walls surround the room, in edit mode these are useful for setting up decorations but you dont want them in play mode as the arena will generate these for you
    {
        ResolveContainers();
        ClearGeneratedWallsAndFloors();

        var walls = new List<Wall>();

        if(generateExternalWalls)
        {
            // create external walls
            if(RoomPosition == RoomPosition.DoorwayHorizontal)
            {
                walls.Add(CreateWall(0,1));
                walls.Add(CreateWall(0,-1));
            }
            else if(RoomPosition == RoomPosition.DoorwayVertical)
            {
                walls.Add(CreateWall(1,0));
                walls.Add(CreateWall(-1,0));
            }
            else
            {
                for(var x = -1; x<=roomSize; x++)
                {
                    for(var y = -1; y<=roomSize; y++)
                    {
                        if(x <0 || x == roomSize || y < 0 || y == roomSize)
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

    private Wall CreateWall(int x, int y)
    {
        var wall = Instantiate(GameSettings.WallPrefab);
        wall.transform.SetParent(wallsContainer);
        wall.transform.localPosition = new Vector3(x,y,0);
        wall.hideFlags = HideFlags.DontSaveInEditor & HideFlags.NotEditable;
        return wall;
    }

    private GameObject CreateFloor(int x, int y)
    {
        var floor = Instantiate(GameSettings.FloorPrefab);
        floor.transform.SetParent(wallsContainer);
        floor.transform.localPosition = new Vector3(x,y,0);
        floor.hideFlags = HideFlags.DontSaveInEditor & HideFlags.NotEditable;

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
        var top = new Vector2(2, 5);
        var right = new Vector2(5, 2);
        var bottom = new Vector2(2, -1);
        var left = new Vector2(0, 2);

        var list = new List<Vector2>();
        switch(RoomPosition)
        {
            case RoomPosition.TopLeft:
                list.Add(right);
                list.Add(bottom);
                break;
            case RoomPosition.TopMiddle:
                list.Add(right);
                list.Add(bottom);
                list.Add(left);
                break;
            case RoomPosition.TopRight:
                list.Add(bottom);
                list.Add(left);
                break;
            case RoomPosition.MiddleLeft:
                list.Add(top);
                list.Add(right);
                list.Add(bottom);
                break;
            case RoomPosition.Center:
                list.Add(top);
                list.Add(right);
                list.Add(bottom);
                list.Add(left);
                break;
            case RoomPosition.MiddleRight:
                list.Add(top);
                list.Add(bottom);
                list.Add(left);
                break;
            case RoomPosition.BottomLeft:
                list.Add(top);
                list.Add(right);
                break;
            case RoomPosition.BottomMiddle:
                list.Add(top);
                list.Add(right);
                list.Add(left);
                break;
            case RoomPosition.BottomRight:
                list.Add(top);
                list.Add(left);
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