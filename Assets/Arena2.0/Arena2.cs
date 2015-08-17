using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arena2: MonoBehaviour 
{
    public GameObject GridSquarePrefab;
    public GameObject HallwayFloorPrefab;
    public GameObject RoomFloorPrefab;

    public int ArenaSizeX = 10;
    public int ArenaSizeY = 10;
    private GridSquareInfo[,] GridStates;

	// Use this for initialization
	void Start () 
    {
        // init gridmap
        GridStates = new GridSquareInfo[ArenaSizeX, ArenaSizeY];

        // set up squares
        for(var x=0; x<ArenaSizeX; x++)
        {
            for(var y=0; y<ArenaSizeY; y++)
            {
                var info = new GridSquareInfo();
                info.State = GridSquareState.Empty;
                GridStates[x,y] = info;
            }
        }

        // create rooms
        var numRooms = 6;
        for(var i=0; i<numRooms; i++)
        {
            CreateRoom();
        }

        // create square tiles
        for(var x=0; x<ArenaSizeX; x++)
        {
            for(var y=0; y<ArenaSizeY; y++)
            {
                var info = GridStates[x,y];
                if(info.State == GridSquareState.Empty)
                {
                    var tile = Instantiate(GridSquarePrefab);
                    tile.transform.position = GridToWorldPosition(x,y);
                }
                else if(info.State == GridSquareState.Hallway)
                {
                    var tile = Instantiate(HallwayFloorPrefab);
                    tile.transform.position = GridToWorldPosition(x,y);
                }
                else if(info.State == GridSquareState.Room)
                {
                    var tile = Instantiate(RoomFloorPrefab);
                    tile.transform.position = GridToWorldPosition(x,y);
                }
            }
        }
	}

    private void CreateRoom()
    {
        var sizex = 5;
        var sizey = 3;
        var posx = Random.Range(0, ArenaSizeX-sizex);
        var posy = Random.Range(0, ArenaSizeY-sizey);

        for(var x=posx; x<posx + sizex; x++)
        {
            for(var y=posy; y<posy + sizey; y++)
            {
                var info = GridStates[x,y];
                info.State = GridSquareState.Room;

                // pick random square to be a door

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
    public GameObject Tile;
}

public enum GridSquareState
{
    Empty,  // nothing here, available space
    Void,   // nothing here but dont allow anything to be put here
    Room,   // space is part of a room
    Hallway // space is part of a hallway
}