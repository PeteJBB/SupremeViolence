using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class Arena : MonoBehaviour 
{
    public int ArenaSizeX = 10;
    public int ArenaSizeY = 10;
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public float WallDensity = 0.25f;

    public GameObject PlayerPrefab;

    private List<GameObject>[,] GridMap;
    private List<GameObject> wallList;

    private static Arena _instance;
    public static Arena Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Arena>();
            }
            return _instance;
        }
    }

    public int Seed = 0;
    private int lastGeneratedSeed;
    private bool regenerateOnNextUpdate = false;
    private Transform generatedStuff;

    public Texture2D WallSkin;

    private bool isFirstUpdate = true;

    // pickup spawning stuff
    private Pickup[] PickupPrefabs;
    public float PickupSpawnRate = 0.05f; // chance that an item spawns every second
    private float lastSpawnCheck = 0;

    // Decorations
    private Decoration[] DecorationPrefabs;

    private MiniMap miniMap;

	void Start () 
    {
        generatedStuff = transform.FindChild("GeneratedStuff");
        miniMap = FindObjectOfType<MiniMap>();

        LoadResources();

        RemovePreviouslyGeneratedArena();
        GenerateArena();
        SpawnPlayers();

        SpawnDecorations();
        SpawnInitialPickups();

	}

    void LoadResources()
    {
        PickupPrefabs = Resources.LoadAll<Pickup>("Pickups");
        DecorationPrefabs = Resources.LoadAll<Decoration>("Decorations");
    }

    void Update () 
    {
        if(isFirstUpdate)
        {
            NotifyAllGridContentsChanged();
            isFirstUpdate = false;
        }

        if (GameBrain.Instance != null && GameBrain.Instance.State == GameState.GameOn && Time.time - lastSpawnCheck > 1)
        {
            if(Random.Range(0f,1f) < PickupSpawnRate)
            {
                SpawnOneRandomPickup();
            }
            lastSpawnCheck = Time.time;
        }
        
        if(!EditorApplication.isPlaying && regenerateOnNextUpdate)
        {
            regenerateOnNextUpdate = false;
            RemovePreviouslyGeneratedArena();
            GenerateArena();
            SpawnPlayers();
            SpawnDecorations();
            SpawnInitialPickups();
        }

        transform.position = new Vector3(0,0,0.1f);
	}

    void LateUpdate()
    {
        // cleanup grid objects that have been destroyed
        for(var x = 0; x < ArenaSizeX; x++)
        {
            for(var y = 0; y < ArenaSizeY; y++)
            {
                if(GridMap[x,y].Any(o => o == null))
                    GridMap[x,y] = GridMap[x,y].Where(o => o != null).ToList();
            }
        }
    }

    void NotifyAllGridContentsChanged () 
    {
        for(var x = 0; x < ArenaSizeX; x++)
        {
            for(var y = 0; y < ArenaSizeY; y++)
            {
                if(miniMap != null)
                    miniMap.GridContentsChanged(x, y, GridMap[x,y]);
            }
        }
    }

    void OnValidate()
    {
        regenerateOnNextUpdate = true;
    }

    private void RemovePreviouslyGeneratedArena()
    {
        // clear previously generated stuff
        var childList = new List<GameObject>();
        for(var i=0; i<generatedStuff.transform.childCount; i++)
        {
            childList.Add(generatedStuff.transform.GetChild(i).gameObject);
        }
            
        childList.ForEach(child => SafeDestroyRecursive(child));

        wallList = new List<GameObject>();
    }

    private void SafeDestroyRecursive(GameObject obj)
    {
        if(obj.transform.childCount > 0)
        {
            var childList = new List<GameObject>();
            for(var i=0; i<obj.transform.childCount; i++)
            {
                childList.Add(obj.transform.GetChild(i).gameObject);
            }
            childList.ForEach(child => SafeDestroyRecursive(child));
        }
        if(EditorApplication.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    private void GenerateArena()
    {
        Random.seed = Seed;
        lastGeneratedSeed = Seed;

        // init gridmap
        GridMap = new List<GameObject>[ArenaSizeX, ArenaSizeY];

        for(var i=0; i<ArenaSizeX; i++)
        {
            for(var j=0; j<ArenaSizeY; j++)
            {
                GridMap[i,j] = new List<GameObject>();
            }
        }

        // create walls
        wallList = new List<GameObject>();

        //top and bottom
        for(var x = -1; x < ArenaSizeX + 1; x++)
        {
            CreateWall(x,ArenaSizeY);
            CreateWall(x,-1);
        }

        // left and right
        for(var y = 0; y < ArenaSizeY; y++)
        {
            CreateWall(ArenaSizeX,y);
            CreateWall(-1,y);
        }

        // spawn walls and floors inside
        for(var gridx = 0; gridx < ArenaSizeX; gridx++)
        {
            for(var gridy = 0; gridy < ArenaSizeY; gridy++)
            {
                // use density to decide if there should be a wall here
                if(Random.Range(0f, 1f) < WallDensity)
                {
                    CreateWall(gridx, gridy);
                }
                else
                {
                    // spawn floor tile
                    var floor = (GameObject)Instantiate(FloorPrefab, GridToWorldPosition(gridx, gridy, transform.position.z), Quaternion.identity);
                    floor.transform.parent = generatedStuff;
                }
            }
        }

        // now loop through walls, turning on the edges which are adjacent to empty space
        foreach(var wall in wallList)
        {
            var gridPos = WorldToGridPosition(wall.transform.position);
            var x = (int)gridPos.x;
            var y = (int)gridPos.y;

            if(!IsThereAWallAt(x - 1, y))
            {
                wall.transform.FindChild("LeftTop").GetComponent<SpriteRenderer>().enabled = true;
                wall.transform.FindChild("LeftBottom").GetComponent<SpriteRenderer>().enabled = true;
            }
            if(!IsThereAWallAt(x + 1, y))
            {
                wall.transform.FindChild("RightTop").GetComponent<SpriteRenderer>().enabled = true;
                wall.transform.FindChild("RightBottom").GetComponent<SpriteRenderer>().enabled = true;
            }
            if(!IsThereAWallAt(x, y + 1))
                wall.transform.FindChild("Top").GetComponent<SpriteRenderer>().enabled = true;
            if(!IsThereAWallAt(x, y - 1))
                wall.transform.FindChild("Bottom").GetComponent<SpriteRenderer>().enabled = true;

            // now do inside corners
            if(IsThereAWallAt(x-1, y) && IsThereAWallAt(x, y+1) && !IsThereAWallAt(x-1, y+1))
                wall.transform.FindChild("TopLeft").GetComponent<SpriteRenderer>().enabled = true;
            if(IsThereAWallAt(x+1, y) && IsThereAWallAt(x, y+1) && !IsThereAWallAt(x+1, y+1))
                wall.transform.FindChild("TopRight").GetComponent<SpriteRenderer>().enabled = true;
            if(IsThereAWallAt(x+1, y) && IsThereAWallAt(x, y-1) && !IsThereAWallAt(x+1, y-1))
                wall.transform.FindChild("BottomRight").GetComponent<SpriteRenderer>().enabled = true;
            if(IsThereAWallAt(x-1, y) && IsThereAWallAt(x, y-1) && !IsThereAWallAt(x-1, y-1))
                wall.transform.FindChild("BottomLeft").GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public bool IsThereAWallAt(int x, int y)
    {
        if(x < 0 || x >= ArenaSizeX || y < 0 || y >= ArenaSizeY)
            return true;

        return GridMap[x,y].Any(o => wallList.Contains(o));
    }

    private GameObject CreateWall(int gridx, int gridy)
    {
        var pos = GridToWorldPosition(gridx, gridy, transform.position.z);
        var wall = (GameObject)Instantiate(WallPrefab, pos, Quaternion.identity);
        wall.transform.parent = generatedStuff;
        wallList.Add(wall);
        
        // set skin
        if(WallSkin != null)
        {
            foreach(var s in wall.GetComponentsInChildren<SkinableSprite>())
            {
                s.SetSkin(WallSkin);
            }
        }

        // set grid pos
        if(gridx >= 0 && gridx < ArenaSizeX && gridy >= 0 && gridy < ArenaSizeY)
        {
            SetGridObject(gridx, gridy, wall);
        }

        return wall;
    }

    private void SpawnPlayers()
    {
        for(var i=0; i<GameBrain.NumberOfPlayers; i++)
        {
            var emptySpots = GetEmptyGridSpots();
            
            if(emptySpots.Count > 0)
            {
                // choose a random spot
                var spot = emptySpots[Random.Range(0,emptySpots.Count)];
                var player = ((GameObject)Instantiate(PlayerPrefab, GridToWorldPosition(spot), Quaternion.identity)).GetComponent<PlayerControl>();
                player.CurrentGridPos = spot;
                player.PlayerIndex = i;
                player.transform.parent = generatedStuff;
                SetGridObject(spot, player.gameObject);

                // set color
                var torso = player.transform.Find("Torso").GetComponent<SpriteRenderer>();
                switch(i)
                {
                    case 0:
                        torso.color = new Color(.2f, .4f, 1);
                        break;
                    case 1:
                        torso.color = Color.red;
                        break;
                    case 2:
                        torso.color = Color.green;
                        break;
                    case 3:
                        torso.color = Color.yellow;
                        break;
                }
            }
        }
    }

    public Vector3 GridToWorldPosition(int gridx, int gridy, float z = 0)
    {
        return new Vector3((-ArenaSizeX / 2f) + gridx + 0.5f, (-ArenaSizeY / 2f) + gridy + 0.5f, z);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPoint, float z = 0)
    {
        return GridToWorldPosition((int)gridPoint.x, (int)gridPoint.y);
    }

    public Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        var gridX = Mathf.Floor(worldPos.x + ArenaSizeX / 2f);
        var gridY = Mathf.Floor(worldPos.y + ArenaSizeY / 2f);
        return new Vector2(gridX, gridY);
    }

    public List<Vector2> GetEmptyGridSpots()
    {
        var list = new List<Vector2>();
        for (int x = 0; x < GridMap.GetLength(0); x += 1) 
        {
            for (int y = 0; y < GridMap.GetLength(1); y += 1) 
            {
                if(GridMap[x,y].Count == 0)
                {
                    list.Add(new Vector2(x,y));
                }
            }
        }

        return list;
    }

    public List<GameObject> GetGridObjects(int x, int y)
    {
        return GridMap[x,y];
    }

    public void SetGridObject(int x, int y, GameObject obj)
    {
        RemoveGridObject(obj);
        if(x < 0 || y < 0 || x >= ArenaSizeX || y >= ArenaSizeY)
        {
            Debug.LogWarningFormat("Cant set grid pos {0}, {1} for object {2}", x, y, obj.name);
            return;
        }

        GridMap[x,y].Add(obj);

        if(miniMap != null)
            miniMap.GridContentsChanged(x, y, GridMap[x,y]);

    }

    public void SetGridObject(Vector2 gridPosition, GameObject obj)
    {
        SetGridObject((int)gridPosition.x, (int)gridPosition.y, obj);
    }

    public void RemoveGridObject(GameObject obj)
    {
        for (int x = 0; x < ArenaSizeX; x++) 
        {
            for (int y = 0; y < ArenaSizeY; y++) 
            {
                var list = GridMap[x,y];
                if(list.Contains(obj))
                {
                    list.Remove(obj);

                    if(miniMap != null)
                        miniMap.GridContentsChanged(x, y, list);

                    return;
                }
            }
        }
    }

    private void SpawnInitialPickups()
    {
        // spawn one of each item at random free location
        var emptySpots = GetEmptyGridSpots();
        for(var i = 0; i< PickupPrefabs.Length; i++)
        {
            if(emptySpots.Count == 0)
            {
                Debug.Log("Ran out of empty places to spawn items");
                break;
            }   
            
            var spot = emptySpots[Random.Range(0,emptySpots.Count)];
            var prefab = PickupPrefabs[i];
            var instance = (Pickup)Instantiate(prefab, GridToWorldPosition(spot), Quaternion.identity);
            instance.transform.parent = generatedStuff;
            SetGridObject(spot, instance.gameObject);
            emptySpots.Remove(spot);
        }
    }

    private void SpawnOneRandomPickup()
    {
        var pickup = PickupPrefabs[Random.Range(0, PickupPrefabs.Length)];
        var emptySpots = Arena.Instance.GetEmptyGridSpots();
        if(emptySpots.Count > 0)
        {
            var spot = emptySpots[Random.Range(0, emptySpots.Count)];
            var instance = (Pickup)Instantiate(pickup, GridToWorldPosition(spot), Quaternion.identity);
            instance.transform.parent = generatedStuff;
            SetGridObject(spot, instance.gameObject);
        }
    }

    private void SpawnDecorations()
    {
        foreach(var prefab in DecorationPrefabs)
        {
            var deco = prefab.GetComponent<Decoration>();
            if(deco != null)
            {
                // check each square to see if the decorations should spawn here
                for (int x = 0; x < ArenaSizeX; x++) 
                {
                    for (int y = 0; y < ArenaSizeY; y++) 
                    {
                        var pos = deco.GetSpawnLocationForGridSquare(x,y, GridMap[x,y]);
                        if(pos.HasValue)
                        {
                            var instance = (GameObject)Instantiate(prefab.gameObject, pos.Value, Quaternion.identity);
                            instance.transform.parent = generatedStuff;
                        }
                    }

                }
            }
        }
    }
}
