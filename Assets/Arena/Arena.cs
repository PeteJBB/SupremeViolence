﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arena: Singleton<Arena>
{
    public GameObject PlayerPrefab;
    public GameObject PickupIconPrefab;
    
    private const int RoomSize = 5; //  rooms are 5x5 grid squares
    public int RoomsAcross = 3;
    public int RoomsDown = 3;
    public bool GenerateOnStartup = false; //<-- this is here so singletons created in other scenes dont suddenly activate

    private GridSquareInfo[,] GridMap;
    public GridContentsChangedEvent OnGridContentsChanged;

    void Awake()
    {
        // singleton should not persist
        DestroyOnLoad = true;

        if(OnGridContentsChanged == null)
            OnGridContentsChanged = new GridContentsChangedEvent();
    }

	// Use this for initialization
	void Start () 
    {
        
        GenerateGridMap();

        if (GenerateOnStartup)
        {
            GenerateRooms();

            SpawnPlayers();
            SpawnInitialPickups();
        }
    }

    private void GenerateRooms()
    {
        // create rooms
        for(var col = 0; col<RoomsAcross; col++)
        {
            for(var row = 0; row<RoomsDown; row++)
            {
                CreateRoom(row, col);
            }
        }
        
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
        for(var x = 0; x<arenaSize.x; x++)
        {
            for(var y = 0; y<arenaSize.y; y++)
            {
                var sq = GridMap[x,y];
                if (sq.Room == null)
                {
                    if (sq.GridSquareType == GridSquareType.Wall)
                    {
                        var wall = Instantiate(wallPrefab).GetComponent<Wall>();
                        wall.transform.SetParent(wallContainer.transform);
                        wall.transform.localPosition = new Vector3(x, y, 0);
                        //wall.gameObject.hideFlags = HideFlags.HideInHierarchy;

                        // walls around the edge should use the skin of the room they are next to
                        Room closestRoom = null;
                        WallSideFlags sides = 0;

                        if (x == 0)
                        {
                            closestRoom = GridMap[x + 1, y].Room;
                            sides = WallSideFlags.Right | WallSideFlags.Top | WallSideFlags.Bottom;
                        }
                        else if (x == arenaSize.x - 1)
                        {
                            closestRoom = GridMap[x - 1, y].Room;
                            sides = WallSideFlags.Left | WallSideFlags.Top | WallSideFlags.Bottom;
                        }
                        else if (y == 0)
                        {
                            closestRoom = GridMap[x, y + 1].Room;
                            sides = WallSideFlags.Top | WallSideFlags.Left | WallSideFlags.Right;
                        }
                        else if (y == arenaSize.y - 1)
                        {
                            closestRoom = GridMap[x, y - 1].Room;
                            sides = WallSideFlags.Bottom | WallSideFlags.Left | WallSideFlags.Right;
                        }

                        if (closestRoom != null)
                        {
                            wall.SetSkin(closestRoom.WallSkin, sides);
                        }

                        wallList.Add(wall);
                    }
                    else if (sq.GridSquareType == GridSquareType.Empty)
                    {
                        var floor = Instantiate(floorPrefab);
                        floor.transform.SetParent(floorContainer.transform);
                        floor.transform.localPosition = new Vector3(x, y, 0);
                        floor.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
            }
        }
	}

    private Vector2 GetRoomPos(int row, int col)
    {
        var x = (col * RoomSize) + col + 1;
        var y = (row * RoomSize) + row + 1;

        return new Vector2(x, y);
    }

    public Vector2 GetArenaSize()
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
                var info = new GridSquareInfo(x, y);

                if (x == 0 || y == 0 || x == arenaSize.x - 1 || y == arenaSize.y - 1)
                    info.GridSquareType = GridSquareType.Wall;
                else
                    info.GridSquareType = GridSquareType.Empty;

                GridMap[x,y] = info;
            }
        }

        //Helper.DebugLogTime("Grid map done.");
    }

    private void CreateRoom(int row, int col)
    {
        var prefab = GameSettings.RoomPrefabs[Random.Range(0, GameSettings.RoomPrefabs.Length)];
        var room = Instantiate<Room>(prefab);

        var pos = GetRoomPos(row, col);
        //var wpos = GridToWorldPosition(pos);
        room.transform.position = pos;

        // update gridsquare infos
        foreach(var sq in room.GetGridSquares())
        {
            var gpos = WorldToGridPosition(sq.transform.position);
            var info = GridMap[(int)gpos.x, (int)gpos.y];
            info.GridSquareType = sq.SquareType;
            info.Room = room;
        }
    }

    public List<GridSquareInfo> GetEmptyGridSquares()
    {
        return GridMap.Cast<GridSquareInfo>().Where(x => x.GridSquareType == GridSquareType.Empty && !x.Objects.Any()).ToList();
    }

    private void SpawnPlayers()
    {
        for(var i=0; i<GameSettings.NumberOfPlayers; i++)
        {
            var emptySquares = GetEmptyGridSquares();
            
            if(emptySquares.Any())
            {
                // choose a random spot
                var spot = emptySquares[Random.Range(0,emptySquares.Count)];
                var player = Instantiate(PlayerPrefab).GetComponent<PlayerControl>();
                player.transform.position = GridToWorldPosition(spot.x, spot.y);
                //player.CurrentGridPos = spot;
                player.PlayerIndex = i;
                //SetGridObject(spot, player.gameObject);

                // set color
                var torso = player.transform.Find("Torso").GetComponent<SpriteRenderer>();
                torso.color = GameState.Players[i].Color;

                var gridTracker = player.GetComponent<GridTrackedObject>();
                gridTracker.MapColor = GameState.Players[i].Color;
            }
        }
    }

    private void SpawnInitialPickups()
    {
        // spawn one of each item at random free location
        var emptySpots = GetEmptyGridSquares();
        for(var i = 0; i< GameSettings.PickupPrefabs.Length; i++)
        {
            if(emptySpots.Count == 0)
            {
                Debug.Log("Ran out of empty places to spawn items");
                break;
            }   
            
            var sq = emptySpots[Random.Range(0,emptySpots.Count)];

            var icon = Instantiate(PickupIconPrefab).GetComponent<PickupIcon>();
            icon.transform.position = GridToWorldPosition(sq.x, sq.y);
            //icon.transform.parent = generatedStuff;
            icon.PickupPrefab = GameSettings.PickupPrefabs[i];

            //SetGridObject(sq, icon.gameObject);
            emptySpots.Remove(sq);
        }
    }

    void OnDrawGizmos()
    {
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
                Helper.DrawGridSquareGizmos(pos, info.GridSquareType);
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
        return GridToWorldPosition((int)gridPoint.x, (int)gridPoint.y, z);
    }
    
    public static Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        var gridX = Mathf.Floor(worldPos.x + 0.5f);
        var gridY = Mathf.Floor(worldPos.y + 0.5f);
        return new Vector2(gridX, gridY);
    }

    public void RegisterTrackableObject(GridTrackedObject obj)
    {
        obj.OnGridPositionChanged.AddListener((oldPos, newPos) => UpdateObjectGridPosition(obj, oldPos, newPos));
    }

    private void UpdateObjectGridPosition(GridTrackedObject obj, Vector2 oldPos, Vector2 newPos)
    {
        if (oldPos.x >= 0 && oldPos.x < GridMap.GetLength(0) && oldPos.y >= 0 && oldPos.y < GridMap.GetLength(1))
        {
            var oldSq = GridMap[(int)oldPos.x, (int)oldPos.y];
            oldSq.Objects.Remove(obj);
            OnGridContentsChanged.Invoke(oldSq);
        }
        if (newPos.x >= 0 && newPos.x < GridMap.GetLength(0) && newPos.y >= 0 && newPos.y < GridMap.GetLength(1))
        {
            var newSq = GridMap[(int)newPos.x, (int)newPos.y];
            newSq.Objects.Add(obj);
            OnGridContentsChanged.Invoke(newSq);
        }
    }

    public GridSquareInfo GetGridSquare(Vector2 gridPos)
    {
        return GetGridSquare((int)gridPos.x, (int)gridPos.y);
    }
    public GridSquareInfo GetGridSquare(int x, int y)
    {
        return GridMap[x, y];
    }
}

public class GridSquareInfo
{
    public readonly int x;
    public readonly int y;

    public GridSquareType GridSquareType;
    public Room Room;
    public List<GridTrackedObject> Objects;

    public GridSquareInfo(int x, int y)
    {
        this.x = x;
        this.y = y;
        Objects = new List<GridTrackedObject>();
    }
}

public class GridContentsChangedEvent : UnityEvent<GridSquareInfo>
{ }