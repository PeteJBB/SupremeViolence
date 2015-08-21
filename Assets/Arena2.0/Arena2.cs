using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arena2: MonoBehaviour 
{
    public GameObject PlayerPrefab;

    private const int RoomSize = 5; //  rooms are 5x5 grid squares

    public int RoomsAcross = 3;
    public int RoomsDown = 3;

    private GridSquareInfo[,] GridMap;

    private static Arena2 _instance;
    public static Arena2 Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Arena2>();
            }
            return _instance;
        }
    }

	// Use this for initialization
	void Start () 
    {
        GenerateGridMap();
        GenerateRooms();

        SpawnPlayers();

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
        for(var x =0; x<arenaSize.x; x++)
        {
            for(var y =0; y<arenaSize.y; y++)
            {
                var sq = GridMap[x,y];
                if (sq.Room == null)
                {
                    if (sq.GridSquareType == GridSquareType.Wall)
                    {
                        var wall = Instantiate(wallPrefab).GetComponent<Wall>();
                        wall.transform.SetParent(wallContainer.transform);
                        wall.transform.localPosition = new Vector3(x, y, 0);
                        wall.gameObject.hideFlags = HideFlags.HideInHierarchy;

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
            info.GridSquareType = sq.SquareType;
            info.Room = room;
        }
    }

    public List<GridSquareInfo> GetEmptyGridSquares()
    {
        return GridMap.Cast<GridSquareInfo>().Where(x => x.GridSquareType == GridSquareType.Empty).ToList();
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
            }
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
    public int x;
    public int y;

    public GridSquareType GridSquareType;
    public Room Room;
    public List<GameObject> Objects;

    public GridSquareInfo(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}