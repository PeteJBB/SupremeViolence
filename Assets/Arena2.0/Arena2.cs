using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arena2: MonoBehaviour 
{
    public GameObject GridSquarePrefab;
    public GameObject HallwayFloorPrefab;
    public GameObject RoomFloorPrefab;

    private int RoomSize = 5; //  rooms are 5x5 grid squares

    public int RoomsAcross = 3;
    public int RoomsDown = 3;

    [HideInInspector]
    public int ArenaSizeX;
    [HideInInspector]
    public int ArenaSizeY;

    private GridSquareInfo[,] GridMap;

    void Awake()
    {
        ArenaSizeX = (RoomsAcross * RoomSize) + (RoomsAcross - 1) + 2;
        ArenaSizeY = (RoomsDown * RoomSize) + (RoomsDown - 1) + 2;
    }

	// Use this for initialization
	void Start () 
    {
        GenerateGridMap();

        // create rooms
        for(var col = 0; col<RoomsAcross; col++)
        {
            for(var row = 0; row<RoomsDown; row++)
            {
                // work out which map position this is
                RoomPosition mapPos;
                if(col < 1)
                {
                    // left
                    if(row < 1) mapPos = RoomPosition.BottomLeft;
                    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopLeft;
                    else mapPos = RoomPosition.MiddleLeft;
                }
                else if(col >= RoomsAcross - 1)
                {
                    // right
                    if(row < 1) mapPos = RoomPosition.BottomRight;
                    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopRight;
                    else mapPos = RoomPosition.MiddleRight;
                }
                else
                {
                    // middle
                    if(row < 1) mapPos = RoomPosition.BottomMiddle;
                    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopMiddle;
                    else mapPos = RoomPosition.Center;
                }

                var x = (col * RoomSize) + col + 1;
                var y = (row * RoomSize) + row + 1;
                CreateRoom(x, y, mapPos);
            }
        }

        // create doorways
        for(var col = 1; col<=RoomsAcross; col++)
        {
            for(var row = 1; row<=RoomsDown; row++)
            {
                if(col < RoomsAcross)
                {
                    var x = (col * RoomSize) + (col - 1) + 1;
                    var y = (row * RoomSize) + (row - 1) - ((int)Mathf.Ceil(RoomSize / 2f)) + 1;
                    CreateRoom(x,y, RoomPosition.DoorwayHorizontal);
                }
                if(row < RoomsDown)
                {
                    var x = (col * RoomSize) + (col - 1) - ((int)Mathf.Ceil(RoomSize / 2f)) + 1;
                    var y = (row * RoomSize) + (row - 1) + 1;
                    CreateRoom(x,y, RoomPosition.DoorwayVertical);
                }
            }
        }

        // loop through remaining grid squares and create walls
        var wallPrefab = Resources.Load<Wall>("Arena/Wall");
        var wallContainer = new GameObject();
        wallContainer.name = "walls";
        wallContainer.transform.SetParent(transform);

        var wallList = new List<Wall>();
        for(var x =0; x<ArenaSizeX; x++)
        {
            for(var y =0; y<ArenaSizeY; y++)
            {
                var sq = GridMap[x,y];
                if(sq.State == GridSquareState.Wall)
                {
                    var wall = Instantiate(wallPrefab).GetComponent<Wall>();
                    wall.transform.SetParent(wallContainer.transform);
                    wall.transform.localPosition = new Vector3(x,y,0);
                    wallList.Add(wall);
                }
            }
        }

        foreach(var wall in wallList)
        {
            wall.UpdateEdges();
        }
	}

    [ContextMenu("Re-GenerateGridMap")]
    private void GenerateGridMap()
    {
        // init gridmap
        GridMap = new GridSquareInfo[ArenaSizeX, ArenaSizeY];
        
        // set up squares
        for(var x=0; x<ArenaSizeX; x++)
        {
            for(var y=0; y<ArenaSizeY; y++)
            {
                var info = new GridSquareInfo();
                info.State = GridSquareState.Wall;
                GridMap[x,y] = info;
            }
        }
    }

    private void CreateRoom(int posx, int posy, RoomPosition mapPos)
    {
        var roomsList = GameSettings.RoomPrefabs.Where(r => r.RoomPosition == mapPos).ToList();
        if(roomsList.Any())
        {
            var prefab = roomsList[Random.Range(0,roomsList.Count)];
            var room = Instantiate<Room>(prefab);

            room.ClearGeneratedWallsAndFloors();
            room.GenerateWallsAndFloors(false);
            var pos = GridToWorldPosition(posx, posy);
            room.transform.position = pos;

            // update gridsquare infos
            foreach(var sq in room.GetGridSquares())
            {
                var gpos = WorldToGridPosition(sq.transform.position);
                var info = GridMap[(int)gpos.x, (int)gpos.y];
                info.State = sq.State;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if(GridMap == null)
            GenerateGridMap();

        for(var x=0; x<ArenaSizeX; x++)
        {
            for(var y=0; y<ArenaSizeY; y++)
            {
                var info = GridMap[x,y];
                var pos = GridToWorldPosition(x,y);
                Helper.DrawGridSquareGizmos(pos, info.State);
            }
        }
    }

    public static Vector3 GridToWorldPosition(int gridx, int gridy, float z = 0)
    {
        return new Vector3(gridx, gridy, z);
    }
    
    public static Vector3 GridToWorldPosition(Vector2 gridPoint, float z = 0)
    {
        return GridToWorldPosition((int)gridPoint.x, (int)gridPoint.y);
    }
    
    public static Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        var gridX = Mathf.Floor(worldPos.x);
        var gridY = Mathf.Floor(worldPos.y);
        return new Vector2(gridX, gridY);
    }
}

public class GridSquareInfo
{
    public GridSquareState State;
    public GridSquare Square;
}