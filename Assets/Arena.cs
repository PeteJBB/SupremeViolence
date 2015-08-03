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

    private List<GameObject>[,] GridMap;

    public static Arena Instance;

    public delegate void GridContentsChangedHandler(int x, int y, List<GameObject> list);
    public event GridContentsChangedHandler OnGridContentsChanged;

    public int Seed = 0;
    private int lastGeneratedSeed;
    private bool regenerateOnNextUpdate = false;

	void Awake () 
    {
        Instance = this;
        GenerateArena();
        SetPlayerStarts();
	}

    void Start () 
    {
        for(var x = 0; x < ArenaSizeX; x++)
        {
            for(var y = 0; y < ArenaSizeY; y++)
            {
                if(OnGridContentsChanged != null)
                    OnGridContentsChanged(x, y, GridMap[x,y]);
            }
        }
    }
	
	void Update () 
    {
        if(!EditorApplication.isPlaying && regenerateOnNextUpdate)
        {
            regenerateOnNextUpdate = false;
            RemovePreviouslyGeneratedArena();
            GenerateArena();
            SetPlayerStarts();
        }

        transform.position = new Vector3(0,0,0.1f);
	}

    void OnValidate()
    {
        regenerateOnNextUpdate = true;
    }

    private void RemovePreviouslyGeneratedArena()
    {
        if(!EditorApplication.isPlaying)
        {
            // clear previously generated stuff
            var childrenToRemove = new List<GameObject>();
            for(var i=0; i<transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if(child.hideFlags == HideFlags.HideAndDontSave)
                {
                    childrenToRemove.Add(child.gameObject);
                }
            }

            foreach(var child in childrenToRemove)
            {
                Object.DestroyImmediate(child, true);
            }
        }
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
        //top
        var topWall = (GameObject)Instantiate(WallPrefab, new Vector3(0, (ArenaSizeY / 2f) + 0.5f, transform.position.z), Quaternion.identity);
        topWall.transform.localScale = new Vector3(ArenaSizeX, 1, 1);
        topWall.transform.parent = transform;
        topWall.hideFlags = HideFlags.HideAndDontSave;

        //bottom
        var bottomWall = (GameObject)Instantiate(WallPrefab, new Vector3(0, (-ArenaSizeY / 2f) - 0.5f, transform.position.z), Quaternion.identity);
        bottomWall.transform.localScale = new Vector3(ArenaSizeX, 1, 1);
        bottomWall.transform.parent = transform;
        bottomWall.hideFlags = HideFlags.HideAndDontSave;

        //left
        var leftWall = (GameObject)Instantiate(WallPrefab, new Vector3((-ArenaSizeX / 2f) - 0.5f, 0, transform.position.z), Quaternion.identity);
        leftWall.transform.localScale = new Vector3(1, ArenaSizeY + 2, 1);
        leftWall.transform.parent = transform;
        leftWall.hideFlags = HideFlags.HideAndDontSave;

        //right
        var rightWall = (GameObject)Instantiate(WallPrefab, new Vector3((ArenaSizeX / 2f) + 0.5f, 0, transform.position.z), Quaternion.identity);
        rightWall.transform.localScale = new Vector3(1, ArenaSizeY + 2, 1);
        rightWall.transform.parent = transform;
        rightWall.hideFlags = HideFlags.HideAndDontSave;

        // spawn walls and floors inside
        for(var gridx = 0; gridx < ArenaSizeX; gridx++)
        {
            for(var gridy = 0; gridy < ArenaSizeY; gridy++)
            {
                // use density to decide if there should be a wall here
                if(Random.Range(0f, 1f) < WallDensity)
                {
                    var wall = (GameObject)Instantiate(WallPrefab, GridToWorldPosition(gridx, gridy, transform.position.z), Quaternion.identity);
                    wall.transform.parent = transform;
                    wall.hideFlags = HideFlags.HideAndDontSave;
                    SetGridObject(gridx, gridy, wall);
                }
                else
                {
                    // spawn floor tile
                    var floor = (GameObject)Instantiate(FloorPrefab, GridToWorldPosition(gridx, gridy, transform.position.z), Quaternion.identity);
                    floor.transform.parent = transform;
                    floor.hideFlags = HideFlags.HideAndDontSave;
                }
            }
        }
    }

    private void SetPlayerStarts()
    {
        // set player positions
        foreach(var player in GameObject.FindObjectsOfType<PlayerControl>())
        {
            var emptySpots = GetEmptyGridSpots();

            if(emptySpots.Count > 0)
            {
                // choose a random spot
                var spot = emptySpots[Random.Range(0,emptySpots.Count)];
                player.transform.position = GridToWorldPosition(spot);
                player.CurrentGridPos = spot;
                SetGridObject(spot, player.gameObject);
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

    public void SetGridObject(int x, int y, GameObject obj)
    {
        RemoveGridObject(obj);
        if(x < 0 || y < 0 || x > ArenaSizeX || y > ArenaSizeY)
        {
            Debug.LogWarningFormat("Cant set grid pos {0}, {1} for object {2}", x, y, obj.name);
            return;
        }

        GridMap[x,y].Add(obj);

        var minimap = GameObject.FindObjectOfType<MiniMap>();

        if(OnGridContentsChanged != null)
            OnGridContentsChanged(x, y, GridMap[x,y]);

    }

    public void SetGridObject(Vector2 gridPosition, GameObject obj)
    {
        SetGridObject((int)gridPosition.x, (int)gridPosition.y, obj);
    }

    public void RemoveGridObject(GameObject obj)
    {
        for (int x = 0; x < GridMap.GetLength(0); x += 1) 
        {
            for (int y = 0; y < GridMap.GetLength(1); y += 1) 
            {
                var list = GridMap[x,y];
                if(list.Contains(obj))
                {
                    list.Remove(obj);

                    if(OnGridContentsChanged != null)
                        OnGridContentsChanged(x, y, list);

                    return;
                }
            }
        }
    }
}
