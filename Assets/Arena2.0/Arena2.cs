using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arena2: MonoBehaviour 
{
    public GameObject GridSquarePrefab;
    public GameObject HallwayFloorPrefab;
    public GameObject RoomFloorPrefab;

    private const int RoomSize = 5; //  rooms are 5x5 grid squares

    public int RoomsAcross = 3;
    public int RoomsDown = 3;

    private GridSquareInfo[,] GridMap;

	// Use this for initialization
	void Start () 
    {
        GenerateGridMap();

        //Helper.DebugLogTime("Generating rooms");

        // create rooms
        for(var col = 0; col<RoomsAcross; col++)
        {
            for(var row = 0; row<RoomsDown; row++)
            {
                // work out which map position this is
                //RoomPosition mapPos;
                //if(col < 1)
                //{
                //    // left
                //    if(row < 1) mapPos = RoomPosition.BottomLeft;
                //    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopLeft;
                //    else mapPos = RoomPosition.MiddleLeft;
                //}
                //else if(col >= RoomsAcross - 1)
                //{
                //    // right
                //    if(row < 1) mapPos = RoomPosition.BottomRight;
                //    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopRight;
                //    else mapPos = RoomPosition.MiddleRight;
                //}
                //else
                //{
                //    // middle
                //    if(row < 1) mapPos = RoomPosition.BottomMiddle;
                //    else if(row >= RoomsDown - 1) mapPos = RoomPosition.TopMiddle;
                //    else mapPos = RoomPosition.Center;
                //}
                CreateRoom(row, col);
            }
        }

        // create doorways
        //for(var col = 1; col<=RoomsAcross; col++)
        //{
        //    for(var row = 1; row<=RoomsDown; row++)
        //    {
        //        if(col < RoomsAcross)
        //        {
        //            var x = (col * RoomSize) + (col - 1) + 1;
        //            var y = (row * RoomSize) + (row - 1) - ((int)Mathf.Ceil(RoomSize / 2f)) + 1;
        //            CreateRoom(x,y, RoomPosition.DoorwayHorizontal);
        //        }
        //        if(row < RoomsDown)
        //        {
        //            var x = (col * RoomSize) + (col - 1) - ((int)Mathf.Ceil(RoomSize / 2f)) + 1;
        //            var y = (row * RoomSize) + (row - 1) + 1;
        //            CreateRoom(x,y, RoomPosition.DoorwayVertical);
        //        }
        //    }
        //}

        
        // loop through remaining grid squares and create walls and floors
        var wallPrefab = Resources.Load<Wall>("Arena/Wall");
        var wallContainer = new GameObject();
        wallContainer.name = "walls";
        wallContainer.transform.SetParent(transform);

        var floorPrefab = Resources.Load<GameObject>("Arena/Floor");
        var floorContainer = new GameObject();
        floorContainer.name = "floors";
        floorContainer.transform.SetParent(transform);

        var wallList = new List<Wall>();
        var arenaSize = GetArenaSize();
        for(var x =0; x<arenaSize.x; x++)
        {
            for(var y =0; y<arenaSize.y; y++)
            {
                var sq = GridMap[x,y];
                if (sq.Room == null)
                {
                    if (sq.State == GridSquareState.Wall)
                    {
                        var wall = Instantiate(wallPrefab).GetComponent<Wall>();
                        wall.transform.SetParent(wallContainer.transform);
                        wall.transform.localPosition = new Vector3(x, y, 0);
                        wall.gameObject.hideFlags = HideFlags.HideInHierarchy;

                        wallList.Add(wall);
                    }
                    else if (sq.State == GridSquareState.Empty)
                    {
                        var floor = Instantiate(floorPrefab);
                        floor.transform.SetParent(floorContainer.transform);
                        floor.transform.localPosition = new Vector3(x, y, 0);
                        floor.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
        }

        //foreach(var wall in wallList)
        //{
        //    wall.UpdateEdges();
        //}

        //Helper.DebugLogTime("Rooms generated");
	}

    private Vector2 GetRoomPos(int row, int col)
    {
        var x = (col * RoomSize) + col + 1;
        var y = (row * RoomSize) + row + 1;

        return new Vector2(x, y);
    }

    private Vector2 GetArenaSize()
    {
        var x = (RoomsAcross * RoomSize) + (RoomsAcross - 1) + 2;
        var y = (RoomsDown * RoomSize) + (RoomsDown - 1) + 2;
        return new Vector2(x, y);
    }

    [ContextMenu("Re-GenerateGridMap")]
    private void GenerateGridMap()
    {
        //Helper.DebugLogTime("Generating arena grid map...");

        // init gridmap
        var arenaSize = GetArenaSize();
        GridMap = new GridSquareInfo[(int)arenaSize.x, (int)arenaSize.y];
        
        // set up squares
        for(var x=0; x<arenaSize.x; x++)
        {
            for(var y=0; y<arenaSize.y; y++)
            {
                var info = new GridSquareInfo();

                if (x == 0 || y == 0 || x == arenaSize.x - 1 || y == arenaSize.y - 1)
                    info.State = GridSquareState.Wall;
                else
                    info.State = GridSquareState.Empty;

                GridMap[x,y] = info;
            }
        }

        //Helper.DebugLogTime("Grid map done.");
    }

    private void CreateRoom(int row, int col)
    {

        var prefab = GameSettings.RoomPrefabs[Random.Range(0, GameSettings.RoomPrefabs.Length)];
        var room = Instantiate<Room>(prefab);

        // generate stuff
        // this will happen automatically when the room Start() method happens
        //room.GenerateWallsAndFloors();

        //var wpos = GridToWorldPosition(pos);
        var pos = GetRoomPos(row, col);
        room.transform.position = pos;

        // update gridsquare infos
        foreach(var sq in room.GetGridSquares())
        {
            var gpos = WorldToGridPosition(sq.transform.position);
            var info = GridMap[(int)gpos.x, (int)gpos.y];
            info.State = sq.State;
            info.Room = room;
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0,0,0,0.3f);

        //var asize = GetArenaSize().ToVector3(0.1f);
        //var offset = new Vector3(-0.5f, -0.5f, 0);
        
        //Gizmos.DrawCube(transform.position + (asize/2) + offset, asize);

        //// draw rooms outline
        //Gizmos.color = new Color(0,0,0,0.5f);
        //for(var row = 0; row<RoomsAcross; row++)
        //{
        //    for(var col = 0; col<RoomsDown; col++)
        //    {
        //        var x = 1 + (row * RoomSize) + row;
        //        var y = 1 + (col * RoomSize) + col;
        //        var pos = new Vector3(x,y,0);
        //        var size = new Vector3(RoomSize, RoomSize, 0.1f);
        //        Gizmos.DrawCube(transform.position + pos + (size/2) + offset, size);
        //    }
        //}

        var asize = GetArenaSize().ToVector3(0.1f);
        var offset = new Vector3(-0.5f, -0.5f, 0);

        Gizmos.color = new Color(1,1,1,0.2f);
        Gizmos.DrawWireCube(transform.position + (asize/2) + offset, asize);

        for(var row = 0; row<RoomsAcross; row++)
        {
            for(var col = 0; col<RoomsDown; col++)
            {
                var pos = GetRoomPos(row, col).ToVector3(0);
                var size = new Vector3(RoomSize, RoomSize, 0.1f);
                Gizmos.DrawWireCube(transform.position + pos + (size/2) + offset, size);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        
        if(GridMap == null)
            GenerateGridMap();
        
        for(var x=0; x<GridMap.GetLength(0); x++)
        {
            for(var y=0; y<GridMap.GetLength(1); y++)
            {
                var info = GridMap[x,y];
                var pos = GridToWorldPosition(x,y);
                Helper.DrawGridSquareGizmos(pos, info.State);
            }
        }

        OnDrawGizmos();
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
    public Room Room;
}