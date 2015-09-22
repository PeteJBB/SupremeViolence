using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arena : Singleton<Arena>
{
    public Vector2 ArenaSize;    

    public GameObject PlayerPrefab;
    public GameObject PickupIconPrefab;

    private GridSquareInfo[,] GridMap;
    public GridContentsChangedEvent OnGridContentsChanged;

    private int idealNumberOfPickups = 4;
    private float pickupSpawnCheckInterval = 7; // seconds
    private float lastPickupSpawnCheckTime;

    void Awake()
    {
        // singleton should not persist
        DestroyOnLoad = true;

        GenerateGridMap();
        
        if (OnGridContentsChanged == null)
            OnGridContentsChanged = new GridContentsChangedEvent();
    }

    // Use this for initialization
    void Start()
    {        
        SpawnPlayers();
        SpawnInitialPickups();
        //if (GenerateOnStartup)
        //{
        //    GenerateRooms();

        //    SpawnPlayers();
        //    SpawnInitialPickups();

        //    lastPickupSpawnCheckTime = Time.time;
        //}
    }

    void Update()
    {
        if (Time.time - lastPickupSpawnCheckTime >= pickupSpawnCheckInterval)
        {
            CheckSpawnRandomPickup();
            lastPickupSpawnCheckTime = Time.time;
        }
    }

    public Vector2 GetArenaSize()
    {
        var size = new Vector2((int)ArenaSize.x, (int)ArenaSize.y);
        return size;
    }

    [ContextMenu("Re-GenerateGridMap")]
    private void GenerateGridMap()
    {
        //Helper.DebugLogTime("Generating arena grid map...");

        // init gridmap
        var arenaSize = GetArenaSize();
        GridMap = new GridSquareInfo[(int)arenaSize.x, (int)arenaSize.y];

        var oobArr = FindObjectsOfType<OutOfBoundsArea>();
        var walls = FindObjectsOfType<Wall>();

        // set up squares
        for (var x = 0; x < arenaSize.x; x++)
        {
            for (var y = 0; y < arenaSize.y; y++)
            {
                var info = new GridSquareInfo(x, y);

                //if (x == 0 || y == 0 || x == arenaSize.x - 1 || y == arenaSize.y - 1)
                //    info.GridSquareType = GridSquareType.Wall;
                //else

                if (OutOfBoundsArea.IsOutOfBounds(x, y, oobArr))
                    info.GridSquareType = GridSquareType.OutOfBounds;
                else if (Wall.IsThereAWallAt(x, y, walls))
                    info.GridSquareType = GridSquareType.Wall;
                else
                    info.GridSquareType = GridSquareType.Empty;

                GridMap[x, y] = info;
                //OnGridContentsChanged.Invoke(info);
            }
        }

        //Helper.DebugLogTime("Grid map done.");
    }

    
    public List<GridSquareInfo> GetEmptyGridSquares()
    {
        return GridMap.Cast<GridSquareInfo>().Where(x => x.GridSquareType == GridSquareType.Empty && !x.Objects.Any()).ToList();
    }

    private void SpawnPlayers()
    {
        var playerSpawns = GameObject.FindObjectsOfType<PlayerSpawn>().ToList();
        var emptySquares = GetEmptyGridSquares();

        for (var i = 0; i < GameSettings.NumberOfPlayers; i++)
        {
            if (playerSpawns.Any())
            {
                // choose a random spawn point
                var spawnPoint = GetValidPlayerSpawn(playerSpawns, emptySquares);

                if (spawnPoint == null)
                {
                    // nowhere to spawn!
                    Debug.LogError("Couldnt find a valid place to spawn player " + i + "!");
                    return;
                }

                var player = Instantiate(PlayerPrefab).GetComponent<PlayerControl>();
                player.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
                player.PlayerIndex = i;

                // set color
                var torso = player.transform.Find("Body/Torso").GetComponent<SpriteRenderer>();
                torso.color = GameState.Players[i].Color;

                var gridTracker = player.GetComponent<GridTrackedObject>();
                gridTracker.MapColor = GameState.Players[i].Color;
            }
        }
    }

    public PlayerSpawn GetValidPlayerSpawn(List<PlayerSpawn> spawns = null, List<GridSquareInfo> emptySquares = null)
    {
        if(spawns == null)
            spawns = GameObject.FindObjectsOfType<PlayerSpawn>().ToList();
     
        if(emptySquares == null)
            emptySquares = GetEmptyGridSquares();

        PlayerSpawn spawnPoint = null;
        while (spawnPoint == null && spawns.Any())
        {
            spawnPoint = spawns[Random.Range(0, spawns.Count)];
            var sq = emptySquares.FirstOrDefault(s => WorldToGridPosition(spawnPoint.transform.position) == new Vector2(s.x, s.y));
            if (sq == null)
            {
                // square isnt empty, look again
                spawns.Remove(spawnPoint);
                spawnPoint = null;
            }
            else
                emptySquares.Remove(sq);
        }

        return spawnPoint;
    }

    private Pickup ChooseRandomPickup()
    {
        // weighted selection by price - so more expensive items spawn less often
        var spawnablePickups = GameSettings.PickupPrefabs
            .Where(x => x.SpawnDuringGame)
            .OrderBy(x => x.Price)
            .ToList();

        // add up all prices
        var max = spawnablePickups.Sum(x => 100f / x.Price);
        var val = Random.Range(0, max);

        // loop through all pickups adding 100f/price until we reach a value higher than val, then we have a winner
        var total = 0f;
        foreach (var p in spawnablePickups)
        {
            total += 100f / p.Price;
            if (total > val)
                return p;
        }

        Debug.LogError("Failed to choose a random pickup??");
        return null;
    }

    private void SpawnInitialPickups()
    {
        // spawn one of each item at random free location
        var emptySpots = GetEmptyGridSquares();
        var spawns = GameObject.FindObjectsOfType<PickupSpawn>().ToList();

        for (var i = 0; i < idealNumberOfPickups; i++)
        {
            var spawnPoint = GetValidPickupSpawn(spawns, emptySpots);
            if (spawnPoint == null)
            {
                Debug.Log("Ran out of empty places to spawn items");
                break;
            }

            var icon = Instantiate(PickupIconPrefab).GetComponent<PickupIcon>();
            icon.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
            icon.PickupPrefab = ChooseRandomPickup();
        }
    }

    /// <summary>
    /// Called periodically to check if a pickup should be spawned - if so then it spawns one
    /// </summary>
    void CheckSpawnRandomPickup()
    {
        if (PickupIconPrefab == null)
            return;

        // how many pickup icons are around right now?
        var icons = FindObjectsOfType<PickupIcon>();
        var chanceToSpawn = Mathf.Lerp(1, 0, icons.Length / (float)idealNumberOfPickups);

        //Helper.DebugLogTime("CheckSpawnRandomPickup? There are " + icons.Length + " icons already in play");
        if (Random.value <= chanceToSpawn)
        {
            var p = ChooseRandomPickup();
            Debug.Log("Spawning pickup: " + p.PickupName);

            var icon = Instantiate(PickupIconPrefab).GetComponent<PickupIcon>();
            icon.PickupPrefab = p;

            var spawnPoint = GetValidPickupSpawn();
            if (spawnPoint != null)
            {
                icon.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
            }
        }
    }

    public PickupSpawn GetValidPickupSpawn(List<PickupSpawn> spawns = null, List<GridSquareInfo> emptySquares = null)
    {
        if(spawns == null)
            spawns = GameObject.FindObjectsOfType<PickupSpawn>().ToList();
     
        if(emptySquares == null)
            emptySquares = GetEmptyGridSquares();

        PickupSpawn spawnPoint = null;
        while (spawnPoint == null && spawns.Any())
        {
            spawnPoint = spawns[Random.Range(0, spawns.Count)];
            var sq = emptySquares.FirstOrDefault(s => WorldToGridPosition(spawnPoint.transform.position) == new Vector2(s.x, s.y));
            if (sq == null)
            {
                // square isnt empty, look again
                spawns.Remove(spawnPoint);
                spawnPoint = null;
            }
            else
                emptySquares.Remove(sq);
        }

        return spawnPoint;
    }

    void OnDrawGizmos()
    {
        var asize = GetArenaSize().ToVector3(0.1f);
        var offset = new Vector3(-0.5f, -0.5f, 0);

        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Gizmos.DrawWireCube(transform.position + (asize / 2) + offset, asize);

        //for (var row = 0; row < RoomsAcross; row++)
        //{
        //    for (var col = 0; col < RoomsDown; col++)
        //    {
        //        var pos = GetRoomPos(row, col).ToVector3(0);
        //        var size = new Vector3(RoomSize, RoomSize, 0.1f);
        //        Gizmos.DrawWireCube(transform.position + pos + (size / 2) + offset, size);
        //    }
        //}
    }

    void OnDrawGizmosSelected()
    {
        //var asize = GetArenaSize().ToVector3(0.1f);
        //var offset = new Vector3(-0.5f, -0.5f, 0);

        //Gizmos.color = new Color(1, 1, 1, 0.2f);
        //Gizmos.DrawWireCube(transform.position + (asize / 2) + offset, asize);

        Gizmos.color = Color.white;
        if (GridMap == null)
            GenerateGridMap();

        if (Helper.IsEditMode())
        {
            for (var x = 0; x < ArenaSize.x; x++)
            {
                for (var y = 0; y < ArenaSize.y; y++)
                {
                    Helper.DrawGridSquareGizmos(new Vector3(x,y,0), GridSquareType.Empty);
                }
            }
        }
        else
        {
            for (var x = 0; x < GridMap.GetLength(0); x++)
            {
                for (var y = 0; y < GridMap.GetLength(1); y++)
                {
                    var info = GridMap[x, y];
                    var pos = GridToWorldPosition(x, y);
                    Helper.DrawGridSquareGizmos(pos, info.GridSquareType);
                }
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
        UpdateObjectGridPosition(obj, new Vector2(-1, -1), obj.CurrentGridPos);
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