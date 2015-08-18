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

    public int ArenaSizeX = 10;
    public int ArenaSizeY = 10;
    private GridSquareInfo[,] GridMap;

	// Use this for initialization
	void Start () 
    {
        GenerateGridMap();

        // create rooms
        for(var x = 0; x<ArenaSizeX; x+= RoomSize)
        {
            for(var y = 0; y<ArenaSizeY; y+=RoomSize)
            {
                // work out which map position this is
                RoomPosition mapPos;
                if(x < RoomSize)
                {
                    // left
                    if(y < RoomSize) mapPos = RoomPosition.BottomLeft;
                    else if(y >= ArenaSizeY - RoomSize) mapPos = RoomPosition.TopLeft;
                    else mapPos = RoomPosition.MiddleLeft;
                }
                else if(x >= ArenaSizeX - RoomSize)
                {
                    // right
                    if(y < RoomSize) mapPos = RoomPosition.BottomRight;
                    else if(y >= ArenaSizeY - RoomSize) mapPos = RoomPosition.TopRight;
                    else mapPos = RoomPosition.MiddleRight;
                }
                else
                {
                    // middle
                    mapPos = RoomPosition.Center;
                }
                CreateRoom(x, y, mapPos);
            }
        }

	}

    [ContextMenu("Re-GenerateGridMap")]
    private void GenerateGridMap()
    {
        Debug.Log("GenerateGridMap");

        // init gridmap
        GridMap = new GridSquareInfo[ArenaSizeX, ArenaSizeY];
        
        // set up squares
        for(var x=0; x<ArenaSizeX; x++)
        {
            for(var y=0; y<ArenaSizeY; y++)
            {
                var info = new GridSquareInfo();
                info.State = GridSquareState.Void;
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

            var pos = GridToWorldPosition(posx, posy);
            room.transform.position = pos;

            // update gridsquare infos
            foreach(var sq in room.GetGridSquares())
            {
                var gpos = WorldToGridPosition(sq.transform.position);
                var info = GridMap[(int)gpos.x, (int)gpos.y];
                info.State = sq.State;
                info.DoorPosition = sq.DoorPosition;
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

                Helper.DrawGridSquareGizmos(pos, info.State, info.DoorPosition);
            }
        }
    }

    public static Vector3 GridToWorldPosition(int gridx, int gridy, float z = 0)
    {
        return new Vector3(gridx + 0.5f, gridy + 0.5f, z);
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
    public DoorPosition DoorPosition;
}