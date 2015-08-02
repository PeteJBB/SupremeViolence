using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Arena : MonoBehaviour 
{
    public Vector2 ArenaSize;
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public float WallDensity = 0.25f;

    private List<GameObject>[,] GridMap;

    public static Arena Instance;

    public delegate void GridContentsChangedHandler(int x, int y, List<GameObject> list);
    public event GridContentsChangedHandler OnGridContentsChanged;

	void Awake () 
    {


        Instance = this;

        // init gridmap
        GridMap = new List<GameObject>[(int)ArenaSize.x, (int)ArenaSize.y];
        for(var i=0; i<GridMap.GetLength(0); i++)
        {
            for(var j=0; j<GridMap.GetLength(1); j++)
            {
                GridMap[i,j] = new List<GameObject>();
            }
        }

	    // create walls
        //top
        var topWall = (GameObject)Instantiate(WallPrefab, new Vector3(0, (ArenaSize.y / 2) + 0.5f, 0), Quaternion.identity);
        topWall.transform.localScale = new Vector3(ArenaSize.x, 1, 1);
        topWall.transform.parent = transform;
        //bottom
        var bottomWall = (GameObject)Instantiate(WallPrefab, new Vector3(0, (-ArenaSize.y / 2) - 0.5f, 0), Quaternion.identity);
        bottomWall.transform.localScale = new Vector3(ArenaSize.x, 1, 1);
        bottomWall.transform.parent = transform;
        //left
        var leftWall = (GameObject)Instantiate(WallPrefab, new Vector3((-ArenaSize.x / 2) - 0.5f, 0, 0), Quaternion.identity);
        leftWall.transform.localScale = new Vector3(1, ArenaSize.y + 2, 1);
        leftWall.transform.parent = transform;
        //right
        var rightWall = (GameObject)Instantiate(WallPrefab, new Vector3((ArenaSize.x / 2) + 0.5f, 0, 0), Quaternion.identity);
        rightWall.transform.localScale = new Vector3(1, ArenaSize.y + 2, 1);
        rightWall.transform.parent = transform;

        // spawn walls and floors inside
        for(var gridx = 0; gridx < ArenaSize.x; gridx++)
        {
            for(var gridy = 0; gridy < ArenaSize.y; gridy++)
            {
                // use density to decide if there should be a wall here
                if(Random.Range(0f, 1f) < WallDensity)
                {
                    var wall = (GameObject)Instantiate(WallPrefab, GridToWorldPosition(gridx, gridy), Quaternion.identity);
                    wall.transform.parent = transform;
                    SetGridObject(gridx, gridy, wall);
                }
                else
                {
                    // spawn floor tile
                    Instantiate(FloorPrefab, GridToWorldPosition(gridx, gridy), Quaternion.identity);
                }
            }
        }

        // set player positions
        foreach(var player in GameObject.FindObjectsOfType<PlayerControl>())
        {
            var emptySpots = GetEmptyGridSpots();
            
            // choose a random spot
            var spot = emptySpots[Random.Range(0,emptySpots.Count)];
            player.transform.position = GridToWorldPosition(spot);
            player.CurrentGridPos = spot;
            SetGridObject(spot, player.gameObject);
        }
	}

    void Start () 
    {
        for(var x = 0; x < ArenaSize.x; x++)
        {
            for(var y = 0; y < ArenaSize.y; y++)
            {
                if(OnGridContentsChanged != null)
                    OnGridContentsChanged(x, y, GridMap[x,y]);
            }
        }
    }
	
	void Update () 
    {
	
	}

    public Vector3 GridToWorldPosition(int gridx, int gridy, float z = 0)
    {
        return new Vector3((-ArenaSize.x / 2) + gridx + 0.5f, (-ArenaSize.y / 2) + gridy + 0.5f, z);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPoint, float z = 0)
    {
        return GridToWorldPosition((int)gridPoint.x, (int)gridPoint.y);
    }

    public Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        var gridX = Mathf.Floor(worldPos.x + ArenaSize.x / 2);
        var gridY = Mathf.Floor(worldPos.y + ArenaSize.y / 2);
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
        if(x < 0 || y < 0 || x > ArenaSize.x || y > ArenaSize.y)
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
