﻿using UnityEngine;
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
    private List<GameObject> wallList;

    public static Arena Instance;

    public delegate void GridContentsChangedHandler(int x, int y, List<GameObject> list);
    public event GridContentsChangedHandler OnGridContentsChanged;

    public int Seed = 0;
    private int lastGeneratedSeed;
    private bool regenerateOnNextUpdate = false;

    private Transform generatedStuff;

	void Awake () 
    {
        Instance = this;
        generatedStuff = transform.FindChild("GeneratedStuff");
        RemovePreviouslyGeneratedArena();
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

            // wall edges
//            if(x > 0 && y >= 0 && y < ArenaSizeY && GridMap[x - 1, y].Count == 0)
//                wall.transform.FindChild("Left").GetComponent<SpriteRenderer>().enabled = true;
//            if(x < ArenaSizeX - 1 && y >= 0 && y < ArenaSizeY && GridMap[x + 1, y].Count == 0)
//                wall.transform.FindChild("Right").GetComponent<SpriteRenderer>().enabled = true;
//            if(y < ArenaSizeY - 1 && x >= 0 && x < ArenaSizeX && GridMap[x, y + 1].Count == 0)
//                wall.transform.FindChild("Top").GetComponent<SpriteRenderer>().enabled = true;
//            if(y > 0 && x >= 0 && x < ArenaSizeX && GridMap[x, y - 1].Count == 0)
//                wall.transform.FindChild("Bottom").GetComponent<SpriteRenderer>().enabled = true;

            if(!IsThereAWallAt(x - 1, y))
                wall.transform.FindChild("Left").GetComponent<SpriteRenderer>().enabled = true;
            if(!IsThereAWallAt(x + 1, y))
                wall.transform.FindChild("Right").GetComponent<SpriteRenderer>().enabled = true;
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

    private bool IsThereAWallAt(int x, int y)
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

        if(gridx >= 0 && gridx < ArenaSizeX && gridy >= 0 && gridy < ArenaSizeY)
        {
            SetGridObject(gridx, gridy, wall);
        }

        wallList.Add(wall);

        return wall;
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
        if(x < 0 || y < 0 || x >= ArenaSizeX || y >= ArenaSizeY)
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
