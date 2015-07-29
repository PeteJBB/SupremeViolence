using UnityEngine;
using System.Collections.Generic;

public class Arena : MonoBehaviour 
{
    public Vector2 ArenaSize;
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public float WallDensity = 0.25f;

    private GameObject[,] GridMap;

    public static Arena Instance;

	void Awake () 
    {
        Instance = this;

        //floor = transform.FindChild("Floor");
        //floor.localScale = new Vector3(ArenaSize.x, 1, ArenaSize.y) / 10f;

        GridMap = new GameObject[(int)ArenaSize.x, (int)ArenaSize.y];

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
                    GridMap[gridx, gridy] = wall;
                }
                else
                {
                    // spawn floor tile
                    Instantiate(FloorPrefab, GridToWorldPosition(gridx, gridy), Quaternion.identity);
                }
            }
        }
	}
	
	void Update () 
    {
	
	}

    public Vector3 GridToWorldPosition(int gridx, int gridy)
    {
        return new Vector3((-ArenaSize.x / 2)+ gridx + 0.5f, (-ArenaSize.y / 2) + gridy + 0.5f, 0);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPoint)
    {
        return GridToWorldPosition((int)gridPoint.x, (int)gridPoint.y);
    }

    public List<Vector2> GetEmptyGridSpots()
    {
        var list = new List<Vector2>();
        for (int x = 0; x < GridMap.GetLength(0); x += 1) 
        {
            for (int y = 0; y < GridMap.GetLength(1); y += 1) 
            {
                if(GridMap[x,y] == null)
                {
                    list.Add(new Vector2(x,y));
                }
            }
        }

        return list;
    }

    public void SetGridObject(int x, int y, GameObject obj)
    {
        GridMap[x,y] = obj;
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
                if(GridMap[x,y] == obj)
                {
                    GridMap[x,y] = null;
                    return;
                }
            }
        }
    }
}
